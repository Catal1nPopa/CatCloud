using Helper.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Helper.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddHelper(this IServiceCollection services, IConfiguration configuration)
        {
            var logPath = Path.Combine(AppContext.BaseDirectory, "Logs", "log-mycloud-.txt");
            services.AddSingleton<FileEncryptionService>();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            return services;
        }
    }
}
