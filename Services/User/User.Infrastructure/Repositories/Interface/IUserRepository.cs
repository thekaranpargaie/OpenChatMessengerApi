using Base.Domain;
using User.Application.DTOs;

namespace User.Infrastructure.Repositories.Interface
{
    public interface IUserRepository : IRepository<Domain.User>
    {
        public Task<UserDto> GetByEmailAsync(string email);
    }
}
