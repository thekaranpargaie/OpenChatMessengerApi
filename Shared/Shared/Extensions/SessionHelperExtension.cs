using BuildingBlocks.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Shared.Extensions
{
    public static class SessionHelperExtension
    {
        public static void RegisterSessionManager(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<SessionManager>();
        }
    }
}
