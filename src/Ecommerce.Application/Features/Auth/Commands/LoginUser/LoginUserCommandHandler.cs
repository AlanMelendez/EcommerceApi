using Ecommerce.Application.Common.Errors;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Common.Models;
using Ecommerce.Application.DTOs.Auth;
using MediatR;

namespace Ecommerce.Application.Features.Auth.Commands.LoginUser;

public sealed class LoginUserCommandHandler
    : IRequestHandler<LoginUserCommand, Result<AuthenticationResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHashingService passwordHashingService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHashingService = passwordHashingService;
        _jwtTokenGenerator = jwtTokenGenerator;
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

        var response = new AuthenticationResponse(
            user.Id,
            user.Email,
            accessToken);

        return Result<AuthenticationResponse>.Success(response);
    }
}