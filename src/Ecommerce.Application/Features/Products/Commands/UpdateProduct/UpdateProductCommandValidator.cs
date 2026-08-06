using FluentValidation;

namespace Ecommerce.Application.Features.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty()
            .WithMessage("Product id is required.");

        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .MaximumLength(150)
            .WithMessage("Product name must not exceed 150 characters.");

        RuleFor(command => command.Description)
            .MaximumLength(1000)
            .WithMessage("Product description must not exceed 1000 characters.");

        RuleFor(command => command.Price)
            .GreaterThan(0)
            .WithMessage("Product price must be greater than zero.");

        RuleFor(command => command.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Product stock cannot be negative.");

        RuleFor(command => command.CategoryId)
            .NotEmpty()
            .WithMessage("Category id is required.");
    }
}
