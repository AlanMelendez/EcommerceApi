namespace Ecommerce.Application.Common.Models;


//The IProductRepository needs a clean object with all filter, pagination, and sorting options.
public sealed record ProductQueryParameters(
    int PageNumber,
    int PageSize,
    string? Search,
    Guid? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? SortBy,
    string? SortDirection);