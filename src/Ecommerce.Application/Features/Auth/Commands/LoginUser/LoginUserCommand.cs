using Ecommerce.Application.Common.Messaging;
using Ecommerce.Application.DTOs.Auth;

namespace Ecommerce.Application.Features.Auth.Commands.LoginUser;

public sealed record LoginUserCommand(
    string Email,
    string Password) : ICommand<AuthenticationResponse>;