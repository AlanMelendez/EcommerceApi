using Ecommerce.Api.Models;
using Ecommerce.Application.Common.Models;

namespace Ecommerce.Api.Factories;

public static class ApiResponseFactory
{
    //A factory is a class that creates objects.
    //Insted of to create objs in many controllers, we create them in one place.
    public static ApiResponse<T> Success<T>(T data, string message)
    {
        return new ApiResponse<T>(
            true,
            message,
            data);
    }

    public static ApiErrorResponse Failure(Error error)
    {
        return new ApiErrorResponse(
            false,
            error.Message,
            error);
    }
}