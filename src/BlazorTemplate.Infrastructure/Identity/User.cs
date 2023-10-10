using Microsoft.AspNetCore.Identity;

namespace BlazorTemplate.Infrastructure.Identity
{
    public class User : IdentityUser
    {
        public bool IsDisabled { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; } = null!;
    }
}
