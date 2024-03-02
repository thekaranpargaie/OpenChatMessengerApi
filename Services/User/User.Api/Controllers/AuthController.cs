using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using User.Application.Models.Request;
using User.Infrastructure;
using User.Infrastructure.Query;

namespace User.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion(Versioning.DefaultApiVersion)]
    public class AuthController : ControllerBase
    {
        private readonly IUserModule _userModule;
        public AuthController(IUserModule userModule)
        {
            _userModule = userModule;
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestModel request)
        {
            return await _userModule.ExecuteQueryAsync(new LoginQuery(request.Email, request.Password));
        }
    }
}
