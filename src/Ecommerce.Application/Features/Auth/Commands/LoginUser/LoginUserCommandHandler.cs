using Ecommerce.Application.Common.Errors;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Common.Models;
using Ecommerce.Application.DTOs.Auth;
using Ecommerce.Domain.Entities;
using MediatR;

namespace Ecommerce.Application.Features.Auth.Commands.LoginUser;

public sealed class LoginUserCommandHandler
    : IRequestHandler<LoginUserCommand, Result<AuthenticationResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHashingService passwordHashingService,
        IJwtTokenGenerator jwtTokenGenerator,
            IRefreshTokenService refreshTokenService,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHashingService = passwordHashingService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenService = refreshTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthenticationResponse>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(
            request.Email,
            cancellationToken);

        if (user is null)
        {
            return Result<AuthenticationResponse>.Failure(AuthErrors.InvalidCredentials);
        }

        var isPasswordValid = _passwordHashingService.VerifyPassword(
            user,
            request.Password);

        if (!isPasswordValid)
        {
            return Result<AuthenticationResponse>.Failure(AuthErrors.InvalidCredentials);
        }

        var accessToken = _jwtTokenGenerator.GenerateToken(user);

        var rawRefreshToken = _refreshTokenService.GenerateToken();

        var refreshTokenHash = _refreshTokenService.HashToken(rawRefreshToken);

        var refreshToken = new RefreshToken(
            user.Id,
            refreshTokenHash,
            _refreshTokenService.GetExpirationDate());

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthenticationResponse(
            user.Id,
            user.Email,
            accessToken,
            rawRefreshToken);

        return Result<AuthenticationResponse>.Success(response);
    }
}