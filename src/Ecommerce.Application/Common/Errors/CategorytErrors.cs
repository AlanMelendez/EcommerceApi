using Ecommerce.Application.Common.Models;

namespace Ecommerce.Application.Common.Errors;

public static class CategoryErrors
{
    public static readonly Error NotFound = new(
        "Category.NotFound",
        "The requested category was not found.");
}