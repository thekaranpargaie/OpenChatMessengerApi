using Base.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace User.Infrastructure.Configuration.Processing
{
    public static class ServiceModuleExtensions
    {
        public static void AddServiceModule(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient(typeof(ICommandHandler<>), typeof(UnitOfWorkCommandHandlerDecorator<>));
            services.AddTransient(typeof(ICommandHandler<,>), typeof(UnitOfWorkCommandHandlerWithResultDecorator<,>));
            services.AddSingleton(AutoMapperConfig.Initialize());
        }
    }
}
