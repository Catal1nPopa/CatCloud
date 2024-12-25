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

            services.AddScoped<CloudDbContext>();

            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                try
                {
                    LoggerHelper.LogInformation("Initializare baza de date");
                    var context = scopedServices.GetRequiredService<CloudDbContext>();
                    //context.Database.EnsureCreated();
                    context.Database.Migrate();
                    LoggerHelper.LogInformation("Initializare finisata");
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
