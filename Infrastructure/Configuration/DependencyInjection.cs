using Domain.Interfaces;
using Helper.Serilog;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Infrastructure.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IPermissionsRepository, PermissionsRepository>();

            services.AddScoped<CloudDbContext>();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                try
                {
                    LoggerHelper.LogInformation("Creare conexiune cu baza de date");
                    var dbContext = serviceProvider.GetRequiredService<CloudDbContext>();
                    dbContext.Database.Migrate();
                    LoggerHelper.LogInformation("Baza de date a fost conectata cu succes");
                }
                catch (Exception ex)
                {
                    LoggerHelper.LogError(ex, $"Eroare la initializarea bazei de date : {ex.Message}");
                }
            }

            return services;
        }
    }
}
