using Ecommerce.Application.Common.Messaging;
using Ecommerce.Application.Common.Models;
using Ecommerce.Application.DTOs.Products;

namespace Ecommerce.Application.Features.Products.Queries.GetProducts;

public sealed record GetProductsQuery(
    int PageNumber,
    int PageSize,
    string? Search,
    Guid? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? SortBy,
    string? SortDirection) : IQuery<PagedResult<ProductResponse>>;