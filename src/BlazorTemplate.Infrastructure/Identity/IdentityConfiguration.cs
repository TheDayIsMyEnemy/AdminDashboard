using Microsoft.EntityFrameworkCore;

namespace BlazorTemplate.Infrastructure.Identity
{
    public static class IdentityConfiguration
    {
        public static void ConfigureIdentity(this ModelBuilder builder)
        {
            builder.Entity<User>(b =>
            {
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                b.Property(u => u.Email).HasMaxLength(Constants.DefaultPropertyLength);
                b.Property(u => u.NormalizedUserName).HasMaxLength(Constants.DefaultPropertyLength);
                b.Property(u => u.UserName).HasMaxLength(Constants.DefaultPropertyLength);
                b.Property(u => u.NormalizedUserName).HasMaxLength(Constants.DefaultPropertyLength);
                b.Property(u => u.PhoneNumber).HasMaxLength(15);
                // b.Property(e => e.AccountStatus)
                //     .HasMaxLength(50)
                //     .HasConversion(
                //         v => v.ToString(),
                //         v => (UserAccountStatus)Enum.Parse(typeof(UserAccountStatus), v))
                //         .IsUnicode(false);
            });

            builder.Entity<Role>(b =>
            {
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                b.Property(r => r.Name).HasMaxLength(Constants.DefaultPropertyLength);
                b.Property(r => r.NormalizedName).HasMaxLength(Constants.DefaultPropertyLength);
            });
        }
    }
}
