using Ecommerce.Application.Common.Errors;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Common.Models;
using Ecommerce.Application.DTOs.Auth;
using Ecommerce.Domain.Entities;
using MediatR;

namespace Ecommerce.Application.Features.Auth.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, Result<AuthenticationResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHashingService passwordHashingService,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork,
         IRefreshTokenService refreshTokenService,
         IRefreshTokenRepository refreshTokenRepository
        )
    {
        _userRepository = userRepository;
        _passwordHashingService = passwordHashingService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
        _refreshTokenService = refreshTokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Result<AuthenticationResponse>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var emailExists = await _userRepository.EmailExistsAsync(
            request.Email,
            cancellationToken);

        if (emailExists)
        {
            return Result<AuthenticationResponse>.Failure(AuthErrors.EmailAlreadyExists);
        }

        var user = new User(
            request.FirstName,
            request.LastName,
            request.Email,
            "TEMPORARY_PASSWORD_HASH");

        var passwordHash = _passwordHashingService.HashPassword(
            user,
            request.Password);

        user.ChangePasswordHash(passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);


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