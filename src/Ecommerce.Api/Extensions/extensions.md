# Extensions Guide for EcommerceApi

This file explains how to use **extension methods** in our .NET API project.

The goal is to keep `Program.cs` clean and make configuration easy to understand after days or weeks.

---

## 1. What is an extension method?

An **extension method** lets us add a method to an existing type without changing that type.

Example:

```csharp
builder.Services.AddApiRateLimiting();
```

`AddApiRateLimiting()` is not part of .NET by default.  
We create it as an extension method.

Simple idea:

```text
Instead of writing a big configuration block in Program.cs,
we move that block to a separate file and call one clean method.
```

---

## 2. Why use extension methods?

Use extension methods to:

- keep `Program.cs` small
- group related configuration
- reuse configuration
- make the code easier to read
- reduce duplicated setup
- make the project easier to maintain

Bad style:

```csharp
// Program.cs has many big blocks of configuration.
builder.Services.AddRateLimiter(...);
builder.Services.AddSwaggerGen(...);
builder.Services.AddCors(...);
builder.Host.UseSerilog(...);
```

Better style:

```csharp
builder.AddApiSerilog();

builder.Services.AddApiRateLimiting();
builder.Services.AddApiSwagger();
builder.Services.AddApiCors(builder.Configuration);
```

---

## 3. The most important rule

There are two main areas in `Program.cs`.

### Before `builder.Build()`

Use this area to **register services**.

```csharp
builder.Services.AddControllers();
builder.Services.AddApiRateLimiting();
builder.Services.AddApiSwagger();
```

### After `builder.Build()`

Use this area to **configure middleware**.

```csharp
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
```

Memory rule:

```text
builder.Services.Add... -> before builder.Build()
app.Use...              -> after builder.Build()
```

If you call `builder.Services.Add...` after `builder.Build()`, you can get this error:

```text
The service collection cannot be modified because it is read-only.
```

---

## 4. Which type should I extend?

Use this table.

| Situation | Extend this type | Method name style | Example |
|---|---|---|---|
| Register normal services | `IServiceCollection` | `Add...` | `services.AddApiRateLimiting()` |
| Need access to `builder.Host` | `WebApplicationBuilder` | `Add...` or `Configure...` | `builder.AddApiSerilog()` |
| Add middleware to request pipeline | `WebApplication` | `Use...` | `app.UseApiSerilogRequestLogging()` |
| Need config values | `IServiceCollection` + `IConfiguration` | `Add...` | `services.AddApiCors(configuration)` |
| Configure old/custom middleware style | `IApplicationBuilder` | `Use...` | `app.UseCustomMiddleware()` |

---

## 5. Variant 1: `IServiceCollection` extension

Use this when you register services.

Examples:

- Rate Limiting
- Swagger
- CORS
- Health Checks
- Redis
- Application services
- Infrastructure services

### Template

```csharp
namespace Ecommerce.Api.Extensions;

public static class SomeFeatureExtensions
{
    public static IServiceCollection AddSomeFeature(
        this IServiceCollection services)
    {
        // Register services here.

        return services;
    }
}
```

### Example: Rate Limiting

File:

```text
src/Ecommerce.Api/Extensions/RateLimitingExtensions.cs
```

```csharp
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
        services.AddRateLimiter(options =>
        {
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode =
                    StatusCodes.Status429TooManyRequests;

                var jsonResponse = new
                {
                    success = false,
                    message = "Too many requests. Please try again later."
                };

                await context.HttpContext.Response.WriteAsJsonAsync(
                    jsonResponse,
                    cancellationToken);
            };

            options.AddPolicy(RateLimitingPolicies.LoginPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString()
                                  ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true
                    }));
        });

        return services;
    }
}
```

Use it in `Program.cs` before `builder.Build()`:

```csharp
builder.Services.AddApiRateLimiting();
```

---

## 6. Variant 2: `WebApplicationBuilder` extension

Use this when you need access to:

```csharp
builder.Host
builder.Configuration
builder.Environment
```

Best example:

```csharp
builder.Host.UseSerilog(...)
```

This is not only a service registration.  
It configures the application host.

### Template

```csharp
namespace Ecommerce.Api.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder AddSomeBuilderFeature(
        this WebApplicationBuilder builder)
    {
        // Configure builder.Host, builder.Configuration, etc.

        return builder;
    }
}
```

### Example: Serilog host configuration

File:

```text
src/Ecommerce.Api/Extensions/SerilogExtensions.cs
```

```csharp
using Serilog;

namespace Ecommerce.Api.Extensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddApiSerilog(
        this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        });

        return builder;
    }
}
```

Use it in `Program.cs` before `builder.Build()`:

```csharp
builder.AddApiSerilog();
```

---

## 7. Variant 3: `WebApplication` extension

Use this when you add middleware after `builder.Build()`.

Examples:

- `app.UseRateLimiter()`
- `app.UseAuthentication()`
- `app.UseAuthorization()`
- `app.UseSerilogRequestLogging()`

### Template

```csharp
namespace Ecommerce.Api.Extensions;

public static class MiddlewareExtensions
{
    public static WebApplication UseSomeMiddleware(
        this WebApplication app)
    {
        // Add middleware here.

        return app;
    }
}
```

### Example: Serilog request logging

Add this to the same `SerilogExtensions.cs` file:

```csharp
using Serilog;

namespace Ecommerce.Api.Extensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddApiSerilog(
        this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        });

        return builder;
    }

    public static WebApplication UseApiSerilogRequestLogging(
        this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        return app;
    }
}
```

Use it in `Program.cs` after `builder.Build()`:

```csharp
app.UseApiSerilogRequestLogging();
```

---

## 8. Variant 4: extension with `IConfiguration`

Use this when configuration comes from `appsettings.json`.

Examples:

- CORS allowed origins
- Redis connection string
- JWT settings
- Swagger options
- External API URLs

### Template

```csharp
public static IServiceCollection AddSomeFeature(
    this IServiceCollection services,
    IConfiguration configuration)
{
    var settings = configuration
        .GetSection("SomeSection")
        .Get<SomeSettings>();

    if (settings is null)
    {
        throw new InvalidOperationException("Some settings are missing.");
    }

    return services;
}
```

### Example: CORS

```csharp
namespace Ecommerce.Api.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddApiCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy("FrontendApp", policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}
```

Use it before `builder.Build()`:

```csharp
builder.Services.AddApiCors(builder.Configuration);
```

Use middleware after `builder.Build()`:

```csharp
app.UseCors("FrontendApp");
```

---

## 9. Variant 5: extension with options delegate

Use this when you want the caller to customize the configuration.

Example:

```csharp
builder.Services.AddApiFeature(options =>
{
    options.Enabled = true;
});
```

### Template

```csharp
public sealed class ApiFeatureOptions
{
    public bool Enabled { get; set; }
}

public static class ApiFeatureExtensions
{
    public static IServiceCollection AddApiFeature(
        this IServiceCollection services,
        Action<ApiFeatureOptions> configureOptions)
    {
        var options = new ApiFeatureOptions();

        configureOptions(options);

        if (!options.Enabled)
        {
            return services;
        }

        // Register services.

        return services;
    }
}
```

Use this only when the feature really needs flexible options.

---

## 10. Variant 6: Infrastructure extension methods

In Clean Architecture, Infrastructure registers technical services.

Examples:

- EF Core
- SQL Server
- Redis
- Repositories
- JWT services
- Password hashing
- Refresh token service

File:

```text
src/Ecommerce.Infrastructure/DependencyInjection.cs
```

Example:

```csharp
public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    IConfiguration configuration)
{
    services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"));
    });

    services.AddScoped<IProductRepository, ProductRepository>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();

    return services;
}
```

Use it in API `Program.cs`:

```csharp
builder.Services.AddInfrastructureServices(builder.Configuration);
```

Important:

> API calls the extension method, but Infrastructure owns the technical implementation.

---

## 11. Variant 7: Application extension methods

Application registers application services.

Examples:

- MediatR
- FluentValidation
- AutoMapper
- Pipeline behaviors

File:

```text
src/Ecommerce.Application/DependencyInjection.cs
```

Example:

```csharp
public static IServiceCollection AddApplicationServices(
    this IServiceCollection services)
{
    var assembly = Assembly.GetExecutingAssembly();

    services.AddMediatR(configuration =>
    {
        configuration.RegisterServicesFromAssembly(assembly);
    });

    services.AddValidatorsFromAssembly(assembly);

    services.AddAutoMapper(configuration => { }, assembly);

    services.AddTransient(
        typeof(IPipelineBehavior<,>),
        typeof(ValidationPipelineBehavior<,>));

    return services;
}
```

Use it in API `Program.cs`:

```csharp
builder.Services.AddApplicationServices();
```

---

## 12. Recommended `Program.cs` style

A clean `Program.cs` should look like this:

```csharp
using Ecommerce.Api.Extensions;
using Ecommerce.Application;
using Ecommerce.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddApiSerilog();

builder.Services.AddControllers();

builder.Services.AddApplicationServices();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddApiAuthorization();

builder.Services.AddApiRateLimiting();

builder.Services.AddApiSwagger();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseApiSerilogRequestLogging();

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

This is easier to read than putting all configuration directly in `Program.cs`.

---

## 13. Middleware order tips

Order matters.

Recommended order for our API:

```csharp
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseCors("FrontendApp");

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
```

Important:

```text
UseAuthentication() must run before UseAuthorization().
```

Reason:

```text
Authentication identifies the user.
Authorization checks what the user can do.
```

Also:

```text
UseCors(), UseAuthentication(), and UseAuthorization() must appear in the correct order when they are used together.
```

---

## 14. Naming conventions

Use these names:

| Purpose | Recommended name |
|---|---|
| Register services | `AddApiFeature` |
| Register Infrastructure | `AddInfrastructureServices` |
| Register Application | `AddApplicationServices` |
| Add middleware | `UseApiFeature` |
| Configure Serilog host | `AddApiSerilog` |
| Configure Swagger | `AddApiSwagger` |
| Configure CORS | `AddApiCors` |
| Configure health checks | `AddApiHealthChecks` |
| Use health checks | `UseApiHealthChecks` |

---

## 15. Where to place extension files

Recommended structure:

```text
src/Ecommerce.Api/
├── Extensions/
│   ├── AuthorizationExtensions.cs
│   ├── CorsExtensions.cs
│   ├── HealthCheckExtensions.cs
│   ├── RateLimitingExtensions.cs
│   ├── SerilogExtensions.cs
│   └── SwaggerExtensions.cs
│
├── Authorization/
│   └── AuthorizationPolicies.cs
│
└── RateLimiting/
    └── RateLimitingPolicies.cs
```

Infrastructure:

```text
src/Ecommerce.Infrastructure/
├── DependencyInjection.cs
├── Authentication/
├── Caching/
└── Persistence/
```

Application:

```text
src/Ecommerce.Application/
└── DependencyInjection.cs
```

---

## 16. Tips for clean extension methods

### Tip 1: Return the same object

For `IServiceCollection`:

```csharp
return services;
```

For `WebApplicationBuilder`:

```csharp
return builder;
```

For `WebApplication`:

```csharp
return app;
```

This allows chaining.

---

### Tip 2: Keep Add and Use separate

Good:

```csharp
builder.Services.AddApiRateLimiting();
app.UseRateLimiter();
```

Bad:

```csharp
builder.Services.AddApiRateLimitingAndUseMiddleware();
```

Reason:

```text
Services are registered before Build.
Middleware is added after Build.
```

---

### Tip 3: Do not hide too much

Extension methods should make code clean, not confusing.

Good:

```csharp
builder.Services.AddApiRateLimiting();
```

Too hidden:

```csharp
builder.Services.AddEverything();
```

`AddEverything()` is not clear.

---

### Tip 4: Validate important settings

Good:

```csharp
if (string.IsNullOrWhiteSpace(settings.ConnectionString))
{
    throw new InvalidOperationException("Redis connection string is missing.");
}
```

This fails early and clearly.

---

### Tip 5: Keep comments simple

Comments should explain why, not only what.

Good:

```csharp
// We use the client IP as the partition key.
// This gives each IP its own request limit.
```

Weak:

```csharp
// Set partition key.
```

---

### Tip 6: Do not log secrets

Never log:

- passwords
- access tokens
- refresh tokens
- secret keys
- connection strings

---

## 17. Common errors and fixes

### Error: Service collection is read-only

Problem:

```csharp
var app = builder.Build();

builder.Services.AddApiRateLimiting(); // Wrong
```

Fix:

```csharp
builder.Services.AddApiRateLimiting();

var app = builder.Build();
```

---

### Error: middleware does not run

Problem:

```csharp
builder.Services.AddRateLimiter();
// but missing app.UseRateLimiter()
```

Fix:

```csharp
builder.Services.AddApiRateLimiting();

var app = builder.Build();

app.UseRateLimiter();
```

---

### Error: `UseAuthentication()` and `UseAuthorization()` are in wrong order

Wrong:

```csharp
app.UseAuthorization();
app.UseAuthentication();
```

Correct:

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

---

### Error: extension method not found

Check:

1. The file namespace.
2. The `using` in `Program.cs`.
3. The method is `public static`.
4. The class is `public static`.
5. The first parameter has `this`.

Example:

```csharp
public static IServiceCollection AddApiRateLimiting(
    this IServiceCollection services)
```

---

## 18. Training-focused rule for this project

For training, we apply each new concept to one representative endpoint when possible.

Examples:

| Concept | Apply to |
|---|---|
| Redis | `GET /api/products/{id}` |
| Rate Limiting | `POST /api/auth/login` |
| Serilog manual log | `POST /api/auth/login` |
| Health Checks | `/health` |
| API Versioning | one controller |
| Unit Test | one handler |
| Integration Test | one endpoint |

Reason:

```text
Learn the concept quickly.
Avoid repetitive code.
Move forward faster.
```

---

## 19. Quick checklist before creating an extension

Ask yourself:

- Is this code making `Program.cs` too long?
- Is this configuration related to one feature?
- Is this service registration or middleware?
- Does it need `builder.Host`?
- Does it need `IConfiguration`?
- Does it belong to API, Application, or Infrastructure?
- Did I place it before or after `builder.Build()` correctly?

---

## 20. References to review later

Search these official topics when you want to go deeper:

- ASP.NET Core Dependency Injection
- ASP.NET Core Middleware
- ASP.NET Core Startup
- Custom ASP.NET Core Middleware
- Serilog.AspNetCore
- Serilog.Settings.Configuration
