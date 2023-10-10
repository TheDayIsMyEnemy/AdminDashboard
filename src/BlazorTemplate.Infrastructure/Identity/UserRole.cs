using Microsoft.AspNetCore.Identity;

namespace BlazorTemplate.Infrastructure.Identity
{
    public class UserRole : IdentityUserRole<string>
    {
        public virtual User User { get; set; } = null!;

        public virtual Role Role { get; set; } = null!;
    }
}
