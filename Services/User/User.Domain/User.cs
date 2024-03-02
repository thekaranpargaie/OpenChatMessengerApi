using Base.Domain;
using Shared.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User.Domain
{
    public class User : OperationLogEntity
    {
        #region Properties
        [Required]
        public Guid UserAccountId { get; set; }
        [ForeignKey("UserAccountId")]
        public UserAccount UserAccount { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string ContactEmail { get; set; }
        public string Image { get; set; }
        public UserStatus Status { get; set; }
        #endregion
        #region Constructors
        public User()
        {
            // Default constructor
        }
        public User(Guid userId, string firstName, string lastName, string email, string image, UserStatus status)
        {
            SetId(userId == Guid.Empty ? Guid.NewGuid() : userId);
            SetIsNew(userId == Guid.Empty);
            SetIsDeleted(false);
            FirstName = firstName;
            LastName = lastName;
            ContactEmail = email;
            Image = image;
            Status = status;
        }
        #endregion
        #region Methods
        public static User Create(string firstName, string lastName, string email, string image, UserStatus status)
        {
            return new User(Guid.Empty, firstName, lastName, email, image, status);
        }
        public void ChangeStatus(UserStatus newStatus)
        {
            Status = newStatus;
        }
        public void Update(string firstName, string lastName, string email, string image)
        {
            FirstName = firstName;
            LastName = lastName;
            ContactEmail = email;
            Image = image;
        }
        public void Delete()
        {
            SetIsDeleted(true);
        }
        #endregion
    }
}