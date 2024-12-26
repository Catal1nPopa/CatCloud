using Application.Interfaces;
using Application.Services;
using Helper.Configuration;
using Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection ApplicationInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserGroupService, UserGroupService>();
            services.AddInfrastructure();
            services.AddHelper(configuration);
            return services;
        }
    }
}
