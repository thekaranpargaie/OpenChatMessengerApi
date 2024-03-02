using Base;
using Base.Contracts;
using Base.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using User.Application.DTOs;
using User.Application.Models;
using User.Infrastructure.Configuration.Resources;
using User.Infrastructure.Repositories.Interface;

namespace User.Infrastructure.Query
{
    public class LoginQuery : IQuery<Result<LoginReponseModel>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public LoginQuery(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
    public class LoginQueryHandler : IQueryHandler<LoginQuery, Result<LoginReponseModel>>
    {
        private readonly IUserRepository _userRepository;
        private readonly string _jwtSecret;
        public LoginQueryHandler(IUserRepository userRepository, IOptions<JwtOption> jwtOption)
        {
            _userRepository = userRepository;
            _jwtSecret = jwtOption.Value.Secret;
        }
        public async Task<Result<LoginReponseModel>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            var validationResult = await ValidateAsync(user, request);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }
            return await GenerateResponseAsync(user);
        }
        async Task<Result<LoginReponseModel>> ValidateAsync(UserDto user, LoginQuery request)
        {
            if (user is null)
            {
                return await Result.FailAsync<LoginReponseModel>(ValidationMessages.UserNotFoundWithEmail);
            }
            else if (!user.UserAccount.Password.Equals(request.Password))
            {
                return await Result.FailAsync<LoginReponseModel>(ValidationMessages.UserPasswordIncorrect);
            }
            else
            {
                return await Result.SuccessAsync<LoginReponseModel>();
            }
        }
        async Task<Result<LoginReponseModel>> GenerateResponseAsync(UserDto user)
        {
            string token = GenerateJwtToken(user);
            return await Result.SuccessAsync(new LoginReponseModel
            {
                Token = token,
                UserData = new UserBasicDetailsModel
                {
                    Id = user.Id,
                    FullName = user.FirstName + " " + user.LastName,
                    Status = user.Status.ToString()
                }
            });
        }
        string GenerateJwtToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, user.ContactEmail),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // Add more claims as needed
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expires in 1 hour, adjust as needed
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
