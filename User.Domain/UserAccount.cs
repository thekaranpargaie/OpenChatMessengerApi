using Base.Domain;
using System.ComponentModel.DataAnnotations;

namespace User.Domain
{
    public class UserAccount : OperationLogEntity
    {
        #region Properties
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
        #endregion
        #region Constructors
        public UserAccount()
        {
            // Default constructor
        }
        public UserAccount(Guid userId, string login, string password)
        {
            SetId(userId == Guid.Empty ? Guid.NewGuid() : userId);
            SetIsNew(userId == Guid.Empty);
            SetIsDeleted(false);
            Login = login;
            Password = password;
        }
        #endregion
        #region Methods
        public UserAccount Create(Guid userId, string login, string password)
        {
            return new UserAccount(userId, login, password);
        }
        public void Delete()
        {
            SetIsDeleted(true);
        }
        #endregion
    }
}
