using Ecommerce.Api.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace Ecommerce.Api.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
    {
        // This method is an extension method for IServiceCollection.
        // It allows us to write this in Program.cs:
        // builder.Services.AddApiRateLimiting();

        services.AddRateLimiter(options =>
        {
            // AddRateLimiter registers the rate limiting configuration.
            // Here we define what happens when a request is rejected.

            options.OnRejected = async (context, cancellationToken) =>
            {
                // This code runs when the client sends too many requests.

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                // 429 means "Too Many Requests".
                // The API is telling the client: "You reached the request limit."

                var jsonResponse = new
                {
                    success = false,
                    message = "Too many requests. Please try again later."
                };
                // This is the response body returned to the client.
                // Important: "requests" was corrected. It was written as "resquests".

                await context.HttpContext.Response.WriteAsJsonAsync(
                    jsonResponse,
                    cancellationToken);
                // This sends the JSON response to the client.
                // cancellationToken helps stop the operation if the request is cancelled.
            };

            options.AddPolicy(RateLimitingPolicies.LoginPolicy, httpContext =>
                // This creates a named rate limiting policy.
                // The policy name is LoginPolicy.
                // We will apply it only to the login endpoint.

                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    // partitionKey defines who receives the limit.
                    // Here we use the client IP address.
                    // Example:
                    // IP 192.168.1.10 has 5 attempts.
                    // IP 192.168.1.20 has another 5 attempts.
                    // If the IP is null, we use "unknown".

                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        // PermitLimit means the maximum number of allowed requests.
                        // In this case: 5 login attempts.

                        Window = TimeSpan.FromMinutes(1),
                        // Window means the time period for the limit.
                        // In this case: 5 attempts every 1 minute.

                        QueueLimit = 0,
                        // QueueLimit means how many extra requests can wait.
                        // We use 0 because we do not want login requests to wait.
                        // Extra requests are rejected immediately.

                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        // This defines the order for queued requests.
                        // Because QueueLimit is 0, it does not affect much here.
                        // But it is required for the configuration.

                        AutoReplenishment = true
                        // AutoReplenishment means the counter resets automatically.
                        // After 1 minute, the client can try again.
                    }
                )
            );
        });

        return services;
        // Returning services allows method chaining.
        // Example:
        // builder.Services
        //     .AddControllers()
        //     .AddApiRateLimiting();
    }
}