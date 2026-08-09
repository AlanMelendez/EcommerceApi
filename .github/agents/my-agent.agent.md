---
name: my-agent
description: Describe what this custom agent does and when to use it.
---

# my-agent


## Project name

Ecommerce API — .NET 10 Clean Architecture Training Project

## Main goal

Build a professional, secure, scalable, and maintainable REST API using .NET 10, ASP.NET Core Web API, Clean Architecture, Entity Framework Core, MediatR, FluentValidation, AutoMapper, JWT, Redis, and production-ready backend practices.

This project is also used to learn backend architecture step by step with simple English explanations.

---

## Architecture style

The project follows **Clean Architecture**.

Clean Architecture means that the code is separated into layers. Each layer has a clear responsibility. The most important rule is:

> Inner layers must not depend on outer layers.

The typical dependency direction is:

```text
API Layer
   ↓
Application Layer
   ↓
Domain Layer

Infrastructure Layer depends on Application and Domain to implement external details.
```

---

## Main layers

### 1. API Layer

The API Layer receives HTTP requests and returns HTTP responses.

Main responsibilities:

- Controllers
- Middlewares
- API response formatting
- Authentication and authorization setup
- Swagger/OpenAPI setup
- CORS
- Rate limiting
- Dependency injection configuration

Important files in this topic:

- `ResultExtensions`
- `ApiResponseFactory`
- `GlobalExceptionHandlingMiddleware`

---

### 2. Application Layer

The Application Layer contains use cases and business application rules.

Main responsibilities:

- Commands
- Queries
- MediatR handlers
- DTOs
- Validators
- Pipeline behaviors
- Application exceptions
- Structured errors
- Interfaces used by Infrastructure

Important files in this topic:

- `Behaviors/ValidationPipelineBehavior`
- `Errors/AuthErrors`
- `Errors/CategoryErrors`
- `Errors/ProductErrors`
- `Exceptions/ValidationException`

---

### 3. Domain Layer

The Domain Layer contains the core business model.

Main responsibilities:

- Entities
- Value objects
- Domain rules
- Domain events
- Domain enums

This layer must not depend on the API, Infrastructure, Entity Framework Core, or external libraries when possible.

---

### 4. Infrastructure Layer

The Infrastructure Layer contains technical implementations.

Main responsibilities:

- Entity Framework Core DbContext
- Repositories
- Unit of Work
- SQL Server access
- Redis cache
- External services
- Email services
- File storage
- Identity/security implementations

---

## Request flow overview

A normal request usually follows this path:

```text
HTTP Request
   ↓
ASP.NET Core Middleware Pipeline
   ↓
GlobalExceptionHandlingMiddleware
   ↓
Controller Action
   ↓
MediatR Send(command/query)
   ↓
ValidationPipelineBehavior
   ↓
Handler
   ↓
Repository / Unit of Work
   ↓
Database
   ↓
Result<T>
   ↓
ResultExtensions.ToActionResult()
   ↓
ApiResponseFactory
   ↓
HTTP Response
```

---

## Validation flow

Validation is executed before the handler runs.

The flow is:

```text
Controller sends Command/Query with MediatR
   ↓
MediatR executes pipeline behaviors
   ↓
ValidationPipelineBehavior checks validators
   ↓
If validation passes:
      Continue to handler
   ↓
If validation fails:
      Throw ValidationException
   ↓
GlobalExceptionHandlingMiddleware catches the exception
   ↓
ApiResponseFactory creates a clean error response
   ↓
Client receives HTTP 400 Bad Request
```

---

## Error handling strategy

The project uses two main error styles:

### 1. Expected business errors

Expected business errors are normal situations that can happen in the application.

Examples:

- Product not found
- Category not found
- Email already exists
- Invalid login credentials
- User is not authorized

These errors should usually be returned using the `Result<T>` pattern.

Example flow:

```text
Handler detects product does not exist
   ↓
Handler returns Result.Failure(ProductErrors.NotFound(productId))
   ↓
Controller receives Result<T>
   ↓
ResultExtensions converts it to IActionResult
   ↓
ApiResponseFactory creates the response
   ↓
Client receives a controlled error response
```

### 2. Unexpected exceptions

Unexpected exceptions are real failures.

Examples:

- Database connection failed
- NullReferenceException
- Unhandled bug
- External service failure

These errors are caught by `GlobalExceptionHandlingMiddleware`.

Example flow:

```text
Unexpected exception happens
   ↓
GlobalExceptionHandlingMiddleware catches it
   ↓
Middleware logs the exception
   ↓
Middleware returns a generic safe error response
   ↓
Client receives HTTP 500 Internal Server Error
```

---

## Important files and responsibilities

### `ResultExtensions`

Purpose:

Convert a `Result<T>` object into an ASP.NET Core `IActionResult`.

This keeps controllers clean.

Controllers should not manually write the same success/error response logic again and again.

---

### `ApiResponseFactory`

Purpose:

Create a standard API response format.

This helps all endpoints return responses with the same structure.

Example response shape:

```json
{
  "success": false,
  "message": "Product not found.",
  "data": null,
  "errors": [
    {
      "code": "Product.NotFound",
      "message": "The product was not found."
    }
  ]
}
```

---

### `GlobalExceptionHandlingMiddleware`

Purpose:

Catch exceptions that were not handled before.

This middleware prevents technical exception details from being exposed to the client.

It should:

- Catch exceptions
- Log the exception
- Map known exceptions to correct HTTP status codes
- Return a safe and standard error response

---

### `ValidationPipelineBehavior`

Purpose:

Run FluentValidation validators before the request reaches the MediatR handler.

This keeps validation outside controllers and outside handlers.

This is useful because:

- Controllers stay clean
- Handlers focus on business logic
- Validation is centralized
- All commands and queries follow the same validation flow

---

### `ValidationException`

Purpose:

Represent validation errors inside the Application Layer.

This exception is thrown when FluentValidation detects invalid input.

The API Layer catches it through the global exception middleware and returns HTTP 400.

---

### `AuthErrors`, `CategoryErrors`, `ProductErrors`

Purpose:

Centralize known application errors.

Instead of writing error strings everywhere, the project uses reusable error definitions.

This improves:

- Consistency
- Maintainability
- Clean Code
- Testing
- API documentation

Example:

```text
ProductErrors.NotFound(productId)
CategoryErrors.NotFound(categoryId)
AuthErrors.InvalidCredentials
```

---

## Rules for this project

### Controllers

Controllers should be thin.

They should:

- Receive HTTP requests
- Send commands or queries using MediatR
- Convert `Result<T>` to `IActionResult`

They should not:

- Contain business logic
- Access DbContext directly
- Build complex error responses manually
- Validate complex rules manually

---

### Handlers

Handlers should contain application use case logic.

They should:

- Execute the requested use case
- Use repositories or Unit of Work
- Return `Result<T>` for expected business errors

They should not:

- Return HTTP responses
- Know about controllers
- Know about middleware
- Expose database details

---

### Validators

Validators should check input rules.

Examples:

- Required fields
- Maximum length
- Valid email format
- Positive price
- Required category id

Validators should not:

- Execute complex business workflows
- Return HTTP responses
- Access controllers

---

### Middleware

Middleware should handle cross-cutting concerns.

Cross-cutting concern means logic that affects many parts of the app.

Examples:

- Global exception handling
- Logging
- Authentication
- Authorization
- Rate limiting
- CORS

---

## Preferred error handling rules

Use `Result<T>` when the error is expected.

Examples:

- Product not found
- Category already exists
- Invalid credentials

Throw exceptions only when the flow cannot continue normally.

Examples:

- Validation failed before handler execution
- Unexpected technical errors
- Serious invalid state

---

## Standard flow for a successful request

```text
1. Client sends HTTP request.
2. Request enters ASP.NET Core middleware pipeline.
3. GlobalExceptionHandlingMiddleware wraps the next steps in try/catch.
4. Controller receives the request.
5. Controller sends a command/query through MediatR.
6. ValidationPipelineBehavior runs validators.
7. If validation passes, the handler runs.
8. Handler executes the use case.
9. Handler returns Result.Success(data).
10. Controller calls result.ToActionResult(this).
11. ResultExtensions calls ApiResponseFactory.
12. API returns HTTP 200, 201, or another success status code.
```

---

## Standard flow for a validation error

```text
1. Client sends invalid HTTP request.
2. Controller sends command/query through MediatR.
3. ValidationPipelineBehavior runs validators.
4. One or more validation errors are found.
5. ValidationPipelineBehavior throws ValidationException.
6. GlobalExceptionHandlingMiddleware catches ValidationException.
7. Middleware creates HTTP 400 Bad Request response.
8. ApiResponseFactory creates standard error response.
9. Client receives validation errors.
```

---

## Standard flow for a business error

```text
1. Client sends valid HTTP request.
2. Validation passes.
3. Handler runs.
4. Handler detects an expected business problem.
5. Handler returns Result.Failure(error).
6. Controller receives the failed Result<T>.
7. Controller calls result.ToActionResult(this).
8. ResultExtensions maps the error to an HTTP status code.
9. ApiResponseFactory creates standard error response.
10. Client receives controlled error response.
```

---

## Standard flow for an unexpected exception

```text
1. Client sends HTTP request.
2. Request reaches controller, handler, repository, or database logic.
3. An unexpected exception happens.
4. GlobalExceptionHandlingMiddleware catches the exception.
5. Middleware logs the exception.
6. Middleware hides internal technical details.
7. ApiResponseFactory creates generic error response.
8. Client receives HTTP 500 Internal Server Error.
```

---

## Development style

Use simple, readable, and explicit code.

Preferred principles:

- Clean Architecture
- SOLID
- Clean Code
- Dependency Injection
- Small classes
- Small methods
- Clear names
- Centralized error handling
- Standard API responses
- Testable use cases

---

## Learning rule

This project must move step by step.

Do not implement many topics at once.

Before writing code:

1. Explain what will be done.
2. Explain why it is done that way.
3. Explain the concepts.
4. Add the required code.
5. Explain how to test it.
6. Explain common errors.
7. Wait for confirmation with `OK`.

