using Ecommerce.Api.Extensions;
using Ecommerce.Api.RateLimiting;
using Ecommerce.Application.Features.Auth.Commands.LoginUser;
using Ecommerce.Application.Features.Auth.Commands.RefreshAccessToken;
using Ecommerce.Application.Features.Auth.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult(this, "User registered successfully.");
    }

    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingPolicies.LoginPolicy)]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult(this, "User logged in successfully.");
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken(
    RefreshAccessTokenCommand command,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult(this, "Token refreshed successfully.");
    }
}