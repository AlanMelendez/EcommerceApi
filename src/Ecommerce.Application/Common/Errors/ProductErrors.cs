using Ecommerce.Application.Common.Models;

namespace Ecommerce.Application.Common.Errors;

public static class ProductErrors
{
    public static readonly Error NotFound = new(
        "Product.NotFound",
        "The requested product was not found.");
}