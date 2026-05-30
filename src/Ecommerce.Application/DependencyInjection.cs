
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(assembly);
            });

            services.AddValidatorsFromAssembly(assembly);

            services.AddAutoMapper(assembly);

            return services;
        }
    }
}
