using Ecommerce.Application.Common.Messaging;

namespace Ecommerce.Application.Features.Products.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : ICommand;
