using Ecommerce.Api.Extensions;
using Ecommerce.Application.Features.Auth.Commands.LoginUser;
using Ecommerce.Application.Features.Auth.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult(this, "User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult(this, "User logged in successfully.");
    }
}