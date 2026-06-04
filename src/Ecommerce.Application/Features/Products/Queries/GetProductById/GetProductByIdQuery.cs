using Ecommerce.Application.Common.Messaging;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Features.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : IQuery<Product>;