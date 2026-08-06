using FluentValidation;

namespace Ecommerce.Application.Features.Products.Queries.GetProducts;

public sealed class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    private static readonly string[] AllowedSortFields =
    [
        "name",
        "price",
        "stock",
        "createdAt"
    ];

    private static readonly string[] AllowedSortDirections =
    [
        "asc",
        "desc"
    ];

    public GetProductsQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("Page size must be between 1 and 50.");

        RuleFor(query => query.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(query => query.MinPrice.HasValue)
            .WithMessage("Minimum price cannot be negative.");

        RuleFor(query => query.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(query => query.MaxPrice.HasValue)
            .WithMessage("Maximum price cannot be negative.");

        RuleFor(query => query)
            .Must(query =>
                !query.MinPrice.HasValue ||
                !query.MaxPrice.HasValue ||
                query.MinPrice.Value <= query.MaxPrice.Value)
            .WithMessage("Minimum price cannot be greater than maximum price.");

        RuleFor(query => query.SortBy)
            .Must(sortBy =>
                string.IsNullOrWhiteSpace(sortBy) ||
                AllowedSortFields.Contains(sortBy))
            .WithMessage("Sort by must be one of: name, price, stock, createdAt.");

        RuleFor(query => query.SortDirection)
            .Must(sortDirection =>
                string.IsNullOrWhiteSpace(sortDirection) ||
                AllowedSortDirections.Contains(sortDirection.ToLowerInvariant()))
            .WithMessage("Sort direction must be asc or desc.");
    }
}