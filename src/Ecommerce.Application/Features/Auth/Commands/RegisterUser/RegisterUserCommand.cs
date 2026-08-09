using Ecommerce.Application.Common.Messaging;
using Ecommerce.Application.DTOs.Auth;

namespace Ecommerce.Application.Features.Auth.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : ICommand<AuthenticationResponse>;