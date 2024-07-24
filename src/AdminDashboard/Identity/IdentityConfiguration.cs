using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Identity
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
            });

            builder.Entity<Role>(b =>
            {
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                b.Property(r => r.Name).HasMaxLength(20);
                b.Property(r => r.NormalizedName).HasMaxLength(20);
            });
        }
    }
}
