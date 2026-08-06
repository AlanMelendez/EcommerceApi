using Ecommerce.Application.Common.Messaging;
using Ecommerce.Application.DTOs.Categories;

namespace Ecommerce.Application.Features.Categories.Queries.GetCategories;

public sealed record GetCategoriesQuery() : IQuery<IReadOnlyList<CategoryResponse>>;
