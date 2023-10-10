using Microsoft.EntityFrameworkCore;

namespace BlazorTemplate.Infrastructure.Identity
{
    public static class IdentityConfiguration
    {
        private const int defaultPropLength = 50;

        public static void ConfigureIdentity(this ModelBuilder builder)
        {
            builder.Entity<User>(b =>
            {
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                b.Property(u => u.Email).HasMaxLength(defaultPropLength);
                b.Property(u => u.NormalizedUserName).HasMaxLength(defaultPropLength);
                b.Property(u => u.UserName).HasMaxLength(defaultPropLength);
                b.Property(u => u.NormalizedUserName).HasMaxLength(defaultPropLength);
                b.Property(u => u.PhoneNumber).HasMaxLength(15);
            });

            builder.Entity<Role>(b =>
            {
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                b.Property(r => r.Name).HasMaxLength(defaultPropLength);
                b.Property(r => r.NormalizedName).HasMaxLength(defaultPropLength);
            });
        }
    }
}
