using Ecommerce.Application.Common.Models;

namespace Ecommerce.Application.Common.Errors;

public static class AuthErrors
{
    public static readonly Error EmailAlreadyExists = new(
        "Auth.EmailAlreadyExists",
        "The email is already registered.");

    public static readonly Error InvalidCredentials = new(
        "Auth.InvalidCredentials",
        "Invalid email or password.");

    public static readonly Error InvalidRefreshToken = new(
        "Auth.InvalidRefreshToken",
        "Invalid refresh token.");
}