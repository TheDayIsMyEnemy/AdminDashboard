using Microsoft.AspNetCore.Identity;

namespace BlazorTemplate.Infrastructure.Identity
{
    public class User : IdentityUser
    {
        public User()
        {

        }

        public User(
            string userName,
            string email,
            UserAccountStatus accountStatus)
        {
            UserName = userName;
            Email = email;
            AccountStatus = accountStatus;
        }

        public UserAccountStatus AccountStatus { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; } = null!;
    }
}
