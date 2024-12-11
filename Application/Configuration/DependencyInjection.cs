using Application.Interfaces;
using Application.Services;
using Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection ApplicationInjection(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddInfrastructure();
            return services;
        }
    }
}
