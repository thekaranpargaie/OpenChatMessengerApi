using Microsoft.AspNetCore.Mvc;

namespace User.Api.Configuration
{
    public static class ServicesConfigurationExtension
    {

        public static IServiceCollection AddVersionSupport(this IServiceCollection services,VersionSettings version)
        {
            services.AddApiVersioning(config =>
            {

                config.DefaultApiVersion = new ApiVersion(majorVersion: version.MajorVersion, minorVersion: version.MinorVersion);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
            return services;
        }
    }

    public class VersionSettings
    {
        public int MinorVersion { get; set; }
        public int MajorVersion { get; set; }
    }
}
