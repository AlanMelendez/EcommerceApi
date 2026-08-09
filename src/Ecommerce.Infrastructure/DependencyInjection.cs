using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Infrastructure.Authentication;
using Ecommerce.Infrastructure.Caching;
using Ecommerce.Infrastructure.Persistence.Context;
using Ecommerce.Infrastructure.Persistence.Repositories;
using Ecommerce.Infrastructure.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var redisConnectionString = configuration.GetConnectionString("Redis");


        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        if (string.IsNullOrWhiteSpace(redisConnectionString))
        {
            throw new InvalidOperationException("Redis connection string is missing.");
        }

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "EcommerceApi:"; // It is a prefix for all keys stored in Redis, to avoid key collisions with other applications using the same Redis instance.
        });
         services.AddScoped<ICacheService, RedisCacheService>();

        services.Configure<JwtSettings>(
         configuration.GetSection(JwtSettings.SectionName)); // Thi connect the config section "Jwt" to out JwtSettings class.

        var jwtSettings = configuration
            .GetSection(JwtSettings.SectionName) 
            .Get<JwtSettings>(); //That create a instance from configuration file.

        if (jwtSettings is null)
        {
            throw new InvalidOperationException("JWT settings are missing.");
        }

        if (jwtSettings.RefreshTokenExpirationDays <= 0)
        {
            throw new InvalidOperationException("Refresh token expiration days must be greater than zero.");
        }


        //Tells our API to use JWT authentication, services.AddAuthentication("Bearer")
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // Use Bearer because we need to send JWT tokens in Authorization header.
           .AddJwtBearer(options =>
           {

               // Configure how tokens are validated
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidIssuer = jwtSettings.Issuer,

                   ValidateAudience = true,
                   ValidAudience = jwtSettings.Audience,

                   ValidateLifetime = true,

                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

                   RoleClaimType = ClaimTypes.Role,

                   ClockSkew = TimeSpan.Zero //•	If token expires at 10:00:00, then at 10:00:01 it is invalid (strict).
               };
           });

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHashingService, PasswordHashingService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();


        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}