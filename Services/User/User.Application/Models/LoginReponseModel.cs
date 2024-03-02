using Shared.Model;

namespace User.Application.Models
{
    public class LoginReponseModel
    {
        public string Token { get; set; }
        public UserBasicDetailsModel UserData { get; set; }
    }
}
