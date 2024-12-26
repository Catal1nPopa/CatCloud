using Domain.Interfaces;
using Helper.Serilog;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();

            services.AddScoped<CloudDbContext>();

            //services.AddDbContext<CloudDbContext>(options =>
            //    options.UseNpgsql("Host=localhost;Port=5432;Database=CatCloud;Username=postgres;Password=admin;IncludeErrorDetail=true"));

            return services;
        }
    }
}
