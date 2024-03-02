using Microsoft.Extensions.DependencyInjection;
using User.Infrastructure.Repositories.Implementation;
using User.Infrastructure.Repositories.Interface;

namespace User.Infrastructure.Repositories
{
    public static class RepositoriesExtension
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
