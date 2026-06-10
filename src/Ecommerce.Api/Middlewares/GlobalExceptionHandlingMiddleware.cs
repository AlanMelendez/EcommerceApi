using System.Net;
using Ecommerce.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Middlewares;

public sealed class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException exception)
        {
            await HandleValidationExceptionAsync(context, exception);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "An unhandled exception occurred. TraceId: {TraceId}",
                context.TraceIdentifier);

            await HandleUnexpectedExceptionAsync(context, exception);
        }
    }

    private static async Task HandleValidationExceptionAsync(
        HttpContext context,
        ValidationException exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Type = "ValidationFailure",
            Title = "Validation error",
            Status = StatusCodes.Status400BadRequest,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        problemDetails.Extensions["errors"] = exception.Errors;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private async Task HandleUnexpectedExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Type = "ServerError",
            Title = "Server error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = _environment.IsDevelopment()
                ? exception.Message
                : "An unexpected error occurred.",
            Instance = context.Request.Path
        };

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}