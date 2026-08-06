using Ecommerce.Application.Common.Messaging;

namespace Ecommerce.Application.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    Guid CategoryId) : ICommand;
