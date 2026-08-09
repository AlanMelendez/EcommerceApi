using Ecommerce.Application.Common.Messaging;
using Ecommerce.Application.DTOs.Auth;

namespace Ecommerce.Application.Features.Auth.Commands.RefreshAccessToken;

public sealed record RefreshAccessTokenCommand(
    string RefreshToken) : ICommand<AuthenticationResponse>;