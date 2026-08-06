using Ecommerce.Application.Common.Messaging;

namespace Ecommerce.Application.Features.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description) : ICommand;