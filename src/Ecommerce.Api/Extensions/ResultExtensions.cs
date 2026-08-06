using Ecommerce.Api.Factories;
using Ecommerce.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(
        this Result<T> result,
        ControllerBase controller,
        string successMessage = "Request completed successfully.")
    {
        if (result.IsSuccess)
        {
            return controller.Ok(
                ApiResponseFactory.Success(result.Value, successMessage));
        }

        return result.Error.Code switch
        {
            "Product.NotFound" => controller.NotFound(ApiResponseFactory.Failure(result.Error)),
            "Category.NotFound" => controller.NotFound(ApiResponseFactory.Failure(result.Error)),

            "Auth.EmailAlreadyExists" => controller.BadRequest(ApiResponseFactory.Failure(result.Error)),
            "Auth.InvalidCredentials" => controller.Unauthorized(ApiResponseFactory.Failure(result.Error)),
            "Auth.InvalidRefreshToken" => controller.Unauthorized(ApiResponseFactory.Failure(result.Error)),

            _ => controller.BadRequest(ApiResponseFactory.Failure(result.Error))
        };
    }

    public static IActionResult ToActionResult(
        this Result result,
        ControllerBase controller,
        string successMessage = "Request completed successfully.")
    {
        if (result.IsSuccess)
        {
            return controller.Ok(
                ApiResponseFactory.Success<object?>(null, successMessage));
        }

        return result.Error.Code switch
        {
            "Product.NotFound" => controller.NotFound(ApiResponseFactory.Failure(result.Error)),
            "Category.NotFound" => controller.NotFound(ApiResponseFactory.Failure(result.Error)),

            "Auth.EmailAlreadyExists" => controller.BadRequest(ApiResponseFactory.Failure(result.Error)),
            "Auth.InvalidCredentials" => controller.Unauthorized(ApiResponseFactory.Failure(result.Error)),
            "Auth.InvalidRefreshToken" => controller.Unauthorized(ApiResponseFactory.Failure(result.Error)),

            _ => controller.BadRequest(ApiResponseFactory.Failure(result.Error))
        };
    }
}