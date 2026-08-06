using System.Globalization;
using Ecommerce.Application.Common.Models;

namespace Ecommerce.Application.Common.Caching;

public static class CacheKeys
{
    public const string ProductsVersion = "products:version";

    public static string ProductById(Guid productId, int version)
    {
        return $"products:v:{version}:id:{productId}";
    }

    public static string ProductList(
        ProductQueryParameters parameters,
        int version)
    {
        return string.Join(
            ":",
            "products",
            "v",
            version,
            "list",
            "page",
            parameters.PageNumber,
            "size",
            parameters.PageSize,
            "search",
            Normalize(parameters.Search),
            "category",
            parameters.CategoryId?.ToString() ?? "all",
            "min",
            FormatDecimal(parameters.MinPrice),
            "max",
            FormatDecimal(parameters.MaxPrice),
            "sort",
            Normalize(parameters.SortBy),
            "direction",
            Normalize(parameters.SortDirection));
    }

    private static string Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? "all"
            : value.Trim().ToLowerInvariant().Replace(" ", "-");
    }

    private static string FormatDecimal(decimal? value)
    {
        return value.HasValue
            ? value.Value.ToString(CultureInfo.InvariantCulture)
            : "all";
    }
}