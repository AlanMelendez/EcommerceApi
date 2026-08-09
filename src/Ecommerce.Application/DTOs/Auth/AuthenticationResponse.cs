namespace Ecommerce.Application.DTOs.Auth;

public sealed record AuthenticationResponse(
    Guid UserId,
    string Email,
    string Role,
    string AccessToken,
    string RefreshToken);