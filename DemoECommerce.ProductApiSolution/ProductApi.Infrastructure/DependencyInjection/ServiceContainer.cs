using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;

namespace ProductApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService (this IServiceCollection services, IConfiguration config)
        {
            // Add database connectivity
            // Add authentication scheme
            SharedServiceContainer.AddSharedLibrary<ProductDbContext>(services, config, config["MySerilog:FileName"]!);

            // Create DI
            services.AddScoped<IProduct, ProductRepository>();
            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            //Register  middleware such as
            // Global Exception: Handle external errors
            // Listen to Only Api Gateway
            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}
