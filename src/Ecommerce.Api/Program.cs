using Ecommerce.Api.Authorization;
using Ecommerce.Api.Middlewares;
using Ecommerce.Api.Services;
using Ecommerce.Application;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Infrastructure;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddSwaggerGen();

// Add authorization policies
builder.Services.AddAuthorizationPolicies();

var app = builder.Build();

// Global error handling middleware.
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
