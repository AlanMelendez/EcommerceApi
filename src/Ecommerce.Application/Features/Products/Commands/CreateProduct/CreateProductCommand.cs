using Ecommerce.Application.Common.Messaging;

namespace Ecommerce.Application.Features.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    Guid CategoryId) : ICommand<Guid>;