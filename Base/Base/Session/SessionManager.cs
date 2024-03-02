using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Shared.Constants;

namespace Base.Session
{
    [ExcludeFromCodeCoverage]
    public class SessionManager
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonName { get; set; }
        public string Browser { get; set; }
        public string IPAddress { get; set; }
        public string TokenKey { get; set; }
        public string UserName { get; private set; }
        public long EmployeeId { get; set; }
        public string Email { get; set; }

        private const string _emailclaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
        private const string _upn = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn";
        private const string _name = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        private const string _surname = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
        private const string _givenname = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
        public SessionManager(IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                if (httpContextAccessor == null || httpContextAccessor.HttpContext == null)
                    return;
                var claims = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
                if (httpContextAccessor.HttpContext.Request.Headers.ContainsKey(ClaimsConstants.UserId))
                {
                    string userid = httpContextAccessor.HttpContext.Request.Headers[ClaimsConstants.UserId]
                        .FirstOrDefault();
                    if (!string.IsNullOrEmpty(userid))
                    {
                        this.UserId = Guid.Parse(userid);
                    }
                }

                if (httpContextAccessor.HttpContext.Request.Headers.ContainsKey(ClaimsConstants.PersonName))
                {
                    string userid = httpContextAccessor.HttpContext.Request.Headers[ClaimsConstants.PersonName]
                        .FirstOrDefault();
                    if (!string.IsNullOrEmpty(PersonName))
                    {
                        this.PersonName = PersonName;
                    }
                }

                if (httpContextAccessor.HttpContext.Request.Headers.ContainsKey(ClaimsConstants.RoleId))
                {
                    string roleid = httpContextAccessor.HttpContext.Request.Headers[ClaimsConstants.RoleId]
                        .FirstOrDefault();
                    if (!string.IsNullOrEmpty(roleid))
                    {
                        this.RoleId = Guid.Parse(roleid);
                    }
                }

                var browser = !string.IsNullOrEmpty(httpContextAccessor.HttpContext.Request.Headers["X-Browser-Name"]
                    .FirstOrDefault())
                    ? httpContextAccessor.HttpContext.Request.Headers["X-Browser-Name"].First()
                    : string.Empty;
                this.Browser = browser;

                if (httpContextAccessor.HttpContext.Connection != null && httpContextAccessor.HttpContext.Connection.RemoteIpAddress != null)
                {
                    this.IPAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                }
                this.TokenKey = httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                //if (preferred_username )
                //****This is where you can set the backend to be anyone by overriding the email ***///
                var email = claims.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
                if (email == null)
                {
                    email = GetEmail(claims.Claims);
                }
                this.UserName = email;
                this.Email = email;
                GetName(claims);
                TokenKey = httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault() ?? string.Empty;
                TokenKey = TokenKey.Replace("Bearer ", string.Empty);
                this.EmployeeId = new Random().Next(1000, 9999);
                //set an id here if you want to test a specific user
                var id = httpContextAccessor.HttpContext.Request.Headers["Id"].FirstOrDefault()?.ToString();
                Guid parsedId;
                Guid.TryParse(id, out parsedId);
                this.UserId = parsedId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in SessionManager", ex);
            }
        }

        private void GetName(ClaimsIdentity claims)
        {
            string firstName = claims.Claims.FirstOrDefault(c => c.Type == _givenname)?.Value;
            string lastName = claims.Claims.FirstOrDefault(c => c.Type == _surname)?.Value;

            if (firstName != null && lastName != null)
            {
                this.FirstName = firstName;
                this.LastName = lastName;
            }
            else
            {
                string name = claims.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                if (name != null)
                {
                    name = name.Replace(",", "");
                    var names = name.Split(' ');
                    if (names.Length > 1)
                    {
                        this.FirstName = names[0];
                        this.LastName = names[1];
                    }
                    else
                    {
                        this.FirstName = names[0];
                        this.LastName = string.Empty;
                    }
                }
            }

        }

        private string? GetEmail(IEnumerable<Claim> claims)
        {
            var value = claims.FirstOrDefault(c => c.Type == _emailclaim)?.Value;
            value ??= claims.FirstOrDefault(c => c.Type == _upn)?.Value;
            value ??= claims.FirstOrDefault(c => c.Type == _name)?.Value;
            return value;
        }
    }
}
