using Ecommerce.Application.Common.Messaging;

namespace Ecommerce.Application.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string? Description) : ICommand<Guid>;