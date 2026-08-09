namespace Ecommerce.Application.Common.Models;

public sealed record ValidationError(
    string PropertyName,
    string ErrorMessage);