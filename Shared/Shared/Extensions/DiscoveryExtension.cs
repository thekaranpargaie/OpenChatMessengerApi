using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Configurations;
using Shared.Utility;

namespace Shared.Extensions
{
    public static class DiscoveryExtension
    {
        public static void RegisterService(this IServiceCollection services, ServiceConfig serviceInformation)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(serviceInformation));
            }

            var consulClient = CreateConsulClient(serviceInformation);

            services.AddSingleton(serviceInformation);
            services.AddSingleton<IHostedService, DiscoveryHostedService>();
            services.AddSingleton<IConsulClient, ConsulClient>(p => consulClient);
        }

        private static ConsulClient CreateConsulClient(ServiceConfig serviceInformation)
        {
            return new ConsulClient(config =>
            {
                config.Address = serviceInformation.ServiceDiscoveryAddress;
            });
        }
    }
}
