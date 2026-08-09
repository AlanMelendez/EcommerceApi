using FluentValidation;

namespace Ecommerce.Application.Features.Auth.Commands.RefreshAccessToken;

public sealed class RefreshAccessTokenCommandValidator
    : AbstractValidator<RefreshAccessTokenCommand>
{
    public RefreshAccessTokenCommandValidator()
    {
        RuleFor(command => command.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required.");
    }
}