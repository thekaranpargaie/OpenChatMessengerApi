using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shared.Configurations.Extension;


namespace Shared.Configuration.Extension
{
    public static class ServicesConfigurationExtension
    {

        public static IServiceCollection AddVersionSupport(this IServiceCollection services, VersionSettings version)
        {
            services.AddApiVersioning(config =>
            {

                config.DefaultApiVersion = new ApiVersion(majorVersion: version.Major, minorVersion: version.Minor);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
            return services;
        }
    }
}