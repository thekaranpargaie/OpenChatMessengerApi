using Shared.Enum;

namespace User.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactEmail { get; set; }
        public string Image { get; set; }
        public UserStatus Status { get; set; }
        public UserAccountDto UserAccount { get; set; }
        public UserDto() { }
        public UserDto(Guid id, string firstName, string lastName, string contactEmail, string image, UserStatus userStatus, string login, string password)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            ContactEmail = contactEmail;
            Image = image;
            Status = userStatus;
            UserAccount = new UserAccountDto
            {
                Login = login,
                Password = password
            };
        }
    }
}
