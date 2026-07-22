using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Infrastructure.Authentication;
using Ecommerce.Infrastructure.Persistence.Context;
using Ecommerce.Infrastructure.Persistence.Repositories;
using Ecommerce.Infrastructure.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Ecommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

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