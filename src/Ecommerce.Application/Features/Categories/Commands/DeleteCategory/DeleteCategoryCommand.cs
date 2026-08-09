using Ecommerce.Application.Common.Messaging;

namespace Ecommerce.Application.Features.Categories.Commands.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid Id) : ICommand;
