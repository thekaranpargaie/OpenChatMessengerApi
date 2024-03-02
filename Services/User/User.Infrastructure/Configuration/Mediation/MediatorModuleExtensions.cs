using Base.Logging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace User.Infrastructure.Configuration.Mediation
{
    public static class MediatorModuleExtensions
    {
        public static void AddMediatorModule(this IServiceCollection services, bool loggingEnabled)
        {
            services.AddMediatR(typeof(MediatorModuleExtensions).Assembly);

            services.AddSingleton<ILogger>(provider =>
            {
                var loggerConfig = new LoggerConfiguration()
                    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                    .WriteTo.Console()
                    .WriteTo.Debug()
                    .CreateLogger();

                return loggerConfig;
            });

            services.AddSingleton<IDebugLogger, DebugLogger>();

            services.AddTransient<ServiceFactory>(provider =>
            {
                var serviceProvider = provider.GetRequiredService<IServiceProvider>();
                return serviceType => serviceProvider.GetService(serviceType);
            });
        }
    }
}