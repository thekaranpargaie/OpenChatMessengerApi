using Base.Domain;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Application.DTOs;
using User.Infrastructure.Configuration.DataAccess.Repository;
using User.Infrastructure.Repositories.Interface;

namespace User.Infrastructure.Repositories.Implementation
{
    public class UserRepository : Repository<Domain.User>, IUserRepository
    {
        public UserRepository(UserDb dbContext) : base(dbContext)
        {
        }

        public async Task<UserDto> GetByEmailAsync(string email)
        {
            return await Table
                .Include(x => x.UserAccount)
                .FirstOrDefaultAsync(x => x.ContactEmail.ToLower() == email.ToLower())
                .Select(x => new UserDto(x.Id, x.FirstName, x.LastName, x.ContactEmail, x.Image, x.Status, x.UserAccount.Login, x.UserAccount.Password));
        }
    }
}
