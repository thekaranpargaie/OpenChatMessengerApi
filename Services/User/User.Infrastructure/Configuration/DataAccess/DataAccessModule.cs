using Base.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Constants;
using User.Infrastructure.Configuration.DataAccess.Repository;

namespace User.Infrastructure.Configuration.DataAccess
{
    public static class DataAccessModuleExtension
    {
        public static void AddDataAccessModule(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<UserDb>(options =>
                options.UseSqlServer(connectionString,
                    sqlOptions => sqlOptions.MigrationsHistoryTable(CommonConstants.DefaultMigration, CommonConstants.User)),
                ServiceLifetime.Scoped);
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IEntityRepository<>), typeof(EntityRepository<>));

        }
    }
}
