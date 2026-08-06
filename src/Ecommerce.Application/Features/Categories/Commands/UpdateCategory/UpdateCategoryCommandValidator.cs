using FluentValidation;

namespace Ecommerce.Application.Features.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty()
            .WithMessage("Category id is required.");

        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(100)
            .WithMessage("Category name must not exceed 100 characters.");

        RuleFor(command => command.Description)
            .MaximumLength(500)
            .WithMessage("Category description must not exceed 500 characters.");
    }
}