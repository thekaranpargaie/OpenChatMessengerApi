using Base.Domain;
using Microsoft.Extensions.DependencyInjection;
using User.Infrastructure.Configuration.DataAccess.Repository;

namespace User.Infrastructure.Configuration.DataAccess
{
    public static class DataAccessModuleExtension
    {
        public static void AddDataAccessModule(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IEntityRepository<>), typeof(EntityRepository<>));
        }
    }
}
