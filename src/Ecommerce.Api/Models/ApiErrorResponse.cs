using Ecommerce.Application.Common.Models;

namespace Ecommerce.Api.Models;

public sealed record ApiErrorResponse(
    bool Success,
    string Message,
    Error Error);