# AGENTS.md

## Project name

**Ecommerce API Training Project**

This project is a step-by-step training project to build a professional REST API using **.NET 10**, **ASP.NET Core Web API**, **Clean Architecture**, **Entity Framework Core**, **SQL Server**, **MediatR**, **FluentValidation**, **AutoMapper**, **JWT**, **refresh tokens**, and later Redis, logging, rate limiting, health checks, API versioning, and tests.

The main goal is not only to create code, but also to understand why each part exists and where it belongs in the architecture.

---

## Language and teaching rules

All explanations, code comments, folder explanations, summaries, and final questions must use **simple English** suitable for a B1/B2 learner.

When the learner writes incorrect English, correct it kindly before answering.

Important teaching rule:

> Before writing code, explain what we are going to do, why we do it this way, and what concepts are involved.

Do not move to the next step until the learner confirms with:

```text
OK
```

Each step must include:

1. Step objective
2. Concepts we will learn
3. Clear and simple explanation
4. Required code
5. Files or folders where the code must be placed
6. How to test that it works
7. Common possible errors
8. Validation checklist
9. Where we are in the flow
10. Final question asking the learner to reply `OK`

---

## Architecture style

The project follows **Clean Architecture**.

### Domain Layer

Project:

```text
src/Ecommerce.Domain
```

Responsibility:

- Business entities
- Business rules
- Enums
- Domain behavior
- No dependency on EF Core, SQL Server, JWT, HTTP, or external frameworks

Examples:

```text
Product
Category
Order
OrderItem
User
RefreshToken
UserRole
OrderStatus
BaseEntity
```

---

### Application Layer

Project:

```text
src/Ecommerce.Application
```

Responsibility:

- Use cases
- Commands
- Queries
- Handlers
- DTOs
- Interfaces
- Validators
- Result Pattern
- MediatR pipeline behaviors
- AutoMapper profiles

Important rule:

> Application defines interfaces, but Infrastructure implements them.

Examples:

```text
IProductRepository
ICategoryRepository
IOrderRepository
IUserRepository
IRefreshTokenRepository
IJwtTokenGenerator
IPasswordHashingService
IRefreshTokenService
IUnitOfWork
```

---

### Infrastructure Layer

Project:

```text
src/Ecommerce.Infrastructure
```

Responsibility:

- EF Core
- SQL Server
- DbContext
- Entity configurations
- Repository implementations
- Unit of Work implementation
- JWT generation
- Password hashing
- Refresh token generation and hashing
- Later Redis, external services, and other technical details

Examples:

```text
AppDbContext
ProductRepository
CategoryRepository
OrderRepository
UserRepository
RefreshTokenRepository
UnitOfWork
JwtTokenGenerator
PasswordHashingService
RefreshTokenService
```

---

### API Layer

Project:

```text
src/Ecommerce.Api
```

Responsibility:

- HTTP endpoints
- Controllers
- Middleware
- Swagger
- CORS
- Rate limiting
- Authorization policies
- Standard API responses
- Error-to-HTTP mapping

Examples:

```text
AuthController
ProductsController
GlobalExceptionHandlingMiddleware
ResultExtensions
ApiResponseFactory
AuthorizationPolicies
Program.cs
```

---

## Completed steps

### Step 0: Define project domain and scope

Selected domain:

```text
Basic E-commerce API
```

Main modules:

- Products
- Categories
- Orders
- Users
- Authentication
- Authorization
- Refresh tokens

The Basic E-commerce domain was selected because it is simple enough to learn, but complete enough to practice real backend architecture.

---

### Step 1: Create solution and project structure

Created a Clean Architecture solution with separated projects:

```text
src/Ecommerce.Domain
src/Ecommerce.Application
src/Ecommerce.Infrastructure
src/Ecommerce.Api
```

Purpose:

- Keep responsibilities separated
- Avoid mixing HTTP, database, and business logic
- Make the project easier to test and maintain

---

### Step 2: Explain Clean Architecture and responsibilities of each layer

Explained the dependency direction:

```text
API → Application → Domain
Infrastructure → Application → Domain
```

Important rule:

> Domain does not depend on any other layer.

Application contains use cases and interfaces.
Infrastructure implements technical details.
API exposes HTTP endpoints.

---

### Step 3: Create domain entities

Created core domain entities:

```text
BaseEntity
Category
Product
Order
OrderItem
OrderStatus
```

Key concepts:

- Encapsulation
- Private setters
- Constructor validation
- Domain behavior inside entities
- Soft delete fields
- Audit fields

Examples of business rules:

- Product name is required
- Product price must be greater than zero
- Product stock cannot be negative
- Order must have items before payment

---

### Step 4: Configure Application Layer

Created Application folders and core abstractions:

```text
Common/Interfaces
Common/Models
DTOs
Features
Mappings
Validators
DependencyInjection.cs
```

Created Result Pattern:

```text
Result
Result<T>
Error
```

Purpose:

- Return success or expected failure without throwing exceptions for normal business cases
- Keep use cases clean
- Avoid using HTTP concepts inside Application

---

### Step 5: Configure Infrastructure Layer

Prepared Infrastructure to implement technical services.

Purpose:

- Keep EF Core and SQL Server outside Application and Domain
- Implement repositories and Unit of Work later
- Centralize technical registrations in Infrastructure Dependency Injection

---

### Step 6: Configure API Layer

Configured API project with:

```text
Controllers
Program.cs
Swagger
Dependency Injection
HTTP pipeline
```

Purpose:

- Expose use cases as HTTP endpoints
- Keep controllers thin
- Send commands and queries to MediatR

---

### Step 7: Configure Entity Framework Core

Created:

```text
AppDbContext
CategoryConfiguration
ProductConfiguration
OrderConfiguration
OrderItemConfiguration
```

Configured:

- Tables
- Keys
- Relationships
- Decimal precision
- Max lengths
- Query filters for soft delete

Purpose:

- Map Domain entities to SQL Server tables without polluting Domain with EF Core attributes

---

### Step 8: Create migrations and database

Created EF Core migrations and updated SQL Server database.

Commands used:

```bash
dotnet ef migrations add InitialCreate --project src/Ecommerce.Infrastructure --startup-project src/Ecommerce.Api --output-dir Persistence/Migrations

dotnet ef database update --project src/Ecommerce.Infrastructure --startup-project src/Ecommerce.Api
```

Purpose:

- Convert entity configurations into real SQL Server tables

---

### Step 9: Implement Repository Pattern

Created repository interfaces in Application and implementations in Infrastructure.

Interfaces:

```text
IProductRepository
ICategoryRepository
IOrderRepository
```

Implementations:

```text
ProductRepository
CategoryRepository
OrderRepository
```

Purpose:

- Application asks for data through abstractions
- Infrastructure handles EF Core details
- Keep handlers clean and testable

---

### Step 10: Implement Unit of Work

Created:

```text
IUnitOfWork
UnitOfWork
```

Purpose:

- Save changes in one controlled operation
- Coordinate database changes from several repositories

Example:

```csharp
await _unitOfWork.SaveChangesAsync(cancellationToken);
```

---

### Step 11: Implement CQRS with MediatR

Created command and query abstractions:

```text
ICommand
ICommand<TResponse>
IQuery<TResponse>
```

Created product use cases:

```text
CreateProductCommand
CreateProductCommandHandler
GetProductByIdQuery
GetProductByIdQueryHandler
```

Purpose:

- Separate write operations from read operations
- Keep controllers thin
- Use MediatR to dispatch requests to handlers

---

### Step 12: Implement DTOs and AutoMapper

Created DTOs:

```text
CategoryResponse
ProductResponse
```

Created:

```text
MappingProfile
```

Purpose:

- Do not expose Domain entities directly to clients
- Return safe and stable response models
- Use AutoMapper to convert entities to DTOs

---

### Step 13: Implement FluentValidation

Created:

```text
CreateProductCommandValidator
ValidationError
ValidationException
ValidationPipelineBehavior
```

Purpose:

- Validate input before the handler runs
- Stop invalid requests early
- Keep validation rules outside controllers

Flow:

```text
Controller → MediatR → ValidationPipelineBehavior → Handler
```

If validation fails, the handler does not execute.

---

### Step 14: Implement global error handling

Created:

```text
GlobalExceptionHandlingMiddleware
```

Purpose:

- Catch exceptions in one central place
- Avoid try/catch in every controller
- Return clean error responses

Handles:

```text
ValidationException → 400 Bad Request
Unexpected exception → 500 Internal Server Error
```

---

### Step 15: Implement standard API responses

Created:

```text
ApiResponse<T>
ApiErrorResponse
ApiResponseFactory
ResultExtensions
```

Purpose:

- Return consistent JSON responses
- Convert Application `Result<T>` into HTTP responses

Normal success example:

```json
{
  "success": true,
  "message": "Request completed successfully.",
  "data": {}
}
```

Normal error example:

```json
{
  "success": false,
  "message": "The requested product was not found.",
  "error": {
    "code": "Product.NotFound",
    "message": "The requested product was not found."
  }
}
```

Important relationship:

```text
Handler returns Result<T>
Controller calls result.ToActionResult(this)
ResultExtensions decides HTTP status
ApiResponseFactory builds the response body
```

---

### Step 16: Implement JWT authentication

Implemented JWT authentication foundation and authentication endpoints.

Created:

```text
User entity
IUserRepository
IJwtTokenGenerator
IPasswordHashingService
JwtSettings
JwtTokenGenerator
PasswordHashingService
UserRepository
UserConfiguration
AuthenticationResponse
AuthErrors
RegisterUserCommand
RegisterUserCommandValidator
RegisterUserCommandHandler
LoginUserCommand
LoginUserCommandValidator
LoginUserCommandHandler
AuthController
```

Purpose:

- Register users
- Hash passwords
- Login users
- Generate JWT access tokens
- Return token to client

Important concepts:

- Authentication means: Who are you?
- JWT is a signed token sent by the client
- Passwords must be hashed, not stored as plain text
- `UseAuthentication()` must run before `UseAuthorization()`

Middleware order:

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

---

### Step 17: Implement refresh tokens

Implemented refresh token support.

Created:

```text
RefreshToken entity
RefreshTokenConfiguration
IRefreshTokenRepository
IRefreshTokenService
RefreshTokenRepository
RefreshTokenService
RefreshAccessTokenCommand
RefreshAccessTokenCommandValidator
RefreshAccessTokenCommandHandler
```

Updated:

```text
AppDbContext
JwtSettings
appsettings.Development.json
AuthenticationResponse
AuthErrors
RegisterUserCommandHandler
LoginUserCommandHandler
AuthController
ResultExtensions
Infrastructure DependencyInjection
```

Purpose:

- Return access token and refresh token
- Store hashed refresh tokens in the database
- Generate a new access token using a valid refresh token
- Revoke the old refresh token after use

Important security decision:

> Store the refresh token hash in the database, not the raw refresh token.

Flow:

```text
Client sends refresh token
API hashes it
API searches by token hash
If valid and active, API creates new tokens
Old refresh token is revoked
New refresh token is stored
```

---

### Step 18: Implement roles and authorization

Implemented role foundation and authorization policies.

Created:

```text
UserRole enum
AuthorizationPolicies
```

Updated:

```text
User entity
UserConfiguration
JwtTokenGenerator
Infrastructure DependencyInjection
Program.cs
AuthenticationResponse
RegisterUserCommandHandler
LoginUserCommandHandler
RefreshAccessTokenCommandHandler
```

Purpose:

- Add a role to each user
- Default new users to `Customer`
- Include role in JWT token
- Configure authorization policies

Roles:

```text
Customer
Admin
```

Policies:

```text
AdminOnly
CustomerOnly
```

Important concepts:

- Authentication means: Who are you?
- Authorization means: What are you allowed to do?
- Role must be included in the JWT token
- ASP.NET Core must know which claim contains the role

Token validation must include:

```csharp
RoleClaimType = ClaimTypes.Role
```

---

## Important response flow

### Normal success or expected failure

```text
Controller
  ↓
MediatR
  ↓
Handler
  ↓
Result<T>
  ↓
ResultExtensions
  ↓
ApiResponseFactory
  ↓
HTTP response
```

### Exception flow

```text
Request
  ↓
GlobalExceptionHandlingMiddleware
  ↓
Controller / MediatR / Handler
  ↓
Exception happens
  ↓
Middleware catches exception
  ↓
ProblemDetails response
```

Important difference:

```text
ResultExtensions handles expected results.
GlobalExceptionHandlingMiddleware handles exceptions.
```

---

## Current project state

Current completed step:

```text
Step 18: Implement roles and authorization
```

Current status:

- JWT authentication exists
- Register and login exist
- Refresh tokens exist
- User roles exist
- JWT includes role claim
- Authorization policies exist

Before moving forward, verify that Step 18 builds and works correctly.

---

## Pending steps

### Step 19: Protect endpoints

Goal:

- Use `[Authorize]`
- Protect endpoints that require login
- Protect admin endpoints using policies
- Test JWT in Swagger

Expected examples:

```csharp
[Authorize]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Authorize(Policy = AuthorizationPolicies.CustomerOnly)]
```

---

### Step 20: Implement complete CRUD

Goal:

- Complete CRUD operations for main entities
- Products
- Categories
- Orders

CRUD means:

```text
Create
Read
Update
Delete
```

---

### Step 21: Implement pagination, filters, and sorting

Goal:

- Add pagination to list endpoints
- Add filtering by fields
- Add sorting
- Add search

Example:

```http
GET /api/products?pageNumber=1&pageSize=10&search=phone&sortBy=name
```

---

### Step 22: Implement Redis cache

Goal:

- Add distributed cache
- Cache frequent queries
- Invalidate cache when data changes

Example:

```text
Cache product list
Clear cache after product update
```

---

### Step 23: Implement rate limiting

Goal:

- Protect API from too many requests
- Add rate limits to sensitive endpoints like login

Example:

```text
Limit login attempts per IP
```

---

### Step 24: Implement logging with Serilog

Goal:

- Add structured logs
- Log important events
- Log errors and request information

---

### Step 25: Implement health checks

Goal:

- Add health endpoint
- Check database connection
- Later check Redis connection

Example:

```http
GET /health
```

---

### Step 26: Configure Swagger correctly

Goal:

- Add JWT support to Swagger
- Allow testing protected endpoints from Swagger
- Improve OpenAPI documentation

---

### Step 27: Configure CORS

Goal:

- Allow specific frontend origins
- Avoid insecure open CORS in production

---

### Step 28: Add API versioning

Goal:

- Support versioned endpoints

Example:

```http
/api/v1/products
/api/v2/products
```

---

### Step 29: Add unit tests

Goal:

- Test handlers
- Test validators
- Mock dependencies

Tools:

```text
xUnit
Moq or NSubstitute
FluentAssertions
```

---

### Step 30: Add integration tests

Goal:

- Test real API behavior
- Test database integration
- Optionally use TestContainers

---

### Step 31: Review Clean Code and refactor

Goal:

- Remove duplication
- Improve naming
- Improve structure
- Review responsibilities

Possible refactor:

- Replace hardcoded error-code switch with better error mapping
- Improve auth token creation flow
- Improve response handling

---

### Step 32: Review general security

Goal:

- Review OWASP API security practices
- Check sensitive data exposure
- Check secure configuration
- Check password and token practices
- Check rate limiting
- Check authorization rules

---

### Step 33: Prepare the project for production

Goal:

- Environment-based configuration
- Production secrets
- Deployment considerations
- Docker optional
- CI/CD considerations

---

### Step 34: Document the project

Goal:

- Create final README
- Explain architecture
- Explain how to run the project
- Explain endpoints
- Explain authentication flow

---

### Step 35: Final summary of architecture and learnings

Goal:

- Review all major concepts
- Review all layers
- Review full request flow
- Review production-readiness checklist

---

## Future extension after core project

After the complete core API is finished, the project may be extended with:

```text
Event-Driven Architecture
Kafka
Microservices
Outbox Pattern
Distributed tracing
Notification service
```

Possible future service:

```text
Ecommerce.NotificationService
```

Possible integration event:

```text
OrderCreated
```

Possible goals:

- Publish events when orders are created
- Consume events in a notification service
- Add retry and idempotency
- Add Outbox Pattern
- Add distributed tracing

---

## Important commands

Build solution:

```bash
dotnet build
```

Run API:

```bash
dotnet run --project src/Ecommerce.Api/Ecommerce.Api.csproj
```

Create migration:

```bash
dotnet ef migrations add MigrationName --project src/Ecommerce.Infrastructure --startup-project src/Ecommerce.Api --output-dir Persistence/Migrations
```

Apply migration:

```bash
dotnet ef database update --project src/Ecommerce.Infrastructure --startup-project src.Ecommerce.Api
```

Correct version of the database update command:

```bash
dotnet ef database update --project src/Ecommerce.Infrastructure --startup-project src/Ecommerce.Api
```

---

## Current next action

Do not move to Step 19 until the learner confirms that Step 18 is clear and working.

Next expected step:

```text
Step 19: Protect endpoints
```

Before writing code for Step 19, explain:

- why endpoints need protection
- difference between `[Authorize]`, roles, and policies
- how JWT is sent in the Authorization header
- how Swagger will be configured later to send JWT
- which endpoints should be public and which should be protected

---

## Rule for future assistants

Always keep the flow.

If the learner asks a question outside the current step:

1. Answer the question clearly.
2. Do not lose the current step.
3. Return to the exact point where the flow stopped.
4. Ask for `OK` before moving forward.

