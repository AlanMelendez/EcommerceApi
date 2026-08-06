using Ecommerce.Application.Common.Messaging;
using Ecommerce.Application.DTOs.Categories;

namespace Ecommerce.Application.Features.Categories.Queries.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id) : IQuery<CategoryResponse>;
