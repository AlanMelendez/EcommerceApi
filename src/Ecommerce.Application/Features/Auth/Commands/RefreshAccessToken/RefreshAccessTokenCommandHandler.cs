using Ecommerce.Application.Common.Errors;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Common.Models;
using Ecommerce.Application.DTOs.Auth;
using Ecommerce.Domain.Entities;
using MediatR;

namespace Ecommerce.Application.Features.Auth.Commands.RefreshAccessToken;

public sealed class RefreshAccessTokenCommandHandler
    : IRequestHandler<RefreshAccessTokenCommand, Result<AuthenticationResponse>>
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshAccessTokenCommandHandler(
        IRefreshTokenService refreshTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenService = refreshTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthenticationResponse>> Handle(
        RefreshAccessTokenCommand request,
        CancellationToken cancellationToken)
    {
        var currentRefreshTokenHash = _refreshTokenService.HashToken(
            request.RefreshToken);

        var currentRefreshToken = await _refreshTokenRepository.GetByTokenHashAsync(
            currentRefreshTokenHash,
            cancellationToken);

        if (currentRefreshToken is null ||
            currentRefreshToken.User is null ||
            !currentRefreshToken.IsActive)
        {
            return Result<AuthenticationResponse>.Failure(AuthErrors.InvalidRefreshToken);
        }

        var user = currentRefreshToken.User;

        var newAccessToken = _jwtTokenGenerator.GenerateToken(user);

        var newRawRefreshToken = _refreshTokenService.GenerateToken();

        var newRefreshTokenHash = _refreshTokenService.HashToken(newRawRefreshToken);

        var newRefreshToken = new RefreshToken(
            user.Id,
            newRefreshTokenHash,
            _refreshTokenService.GetExpirationDate());

        currentRefreshToken.Revoke(newRefreshTokenHash);

        _refreshTokenRepository.Update(currentRefreshToken);

        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthenticationResponse(
            user.Id,
            user.Email,
            newAccessToken,
            newRawRefreshToken);

        return Result<AuthenticationResponse>.Success(response);
    }
}