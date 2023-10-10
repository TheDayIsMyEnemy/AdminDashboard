using BlazorTemplate.Domain.Models;
using BlazorTemplate.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorTemplate.Infrastructure.Data
{
    public class AppDbContext
        : IdentityDbContext<
            User,
            Role,
            string,
            IdentityUserClaim<string>,
            UserRole,
            IdentityUserLogin<string>,
            IdentityRoleClaim<string>,
            IdentityUserToken<string>
        >
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<MemberActivityLog> UserActivityLogs => Set<MemberActivityLog>();
        public DbSet<MemberIPConstraint> UserIPConstraints => Set<MemberIPConstraint>();


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureIdentity();
        }

        // public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        // {
        //     foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        //     {
        //         switch (entry.State)
        //         {
        //             case EntityState.Added:
        //                 //entry.Entity.CreatedBy = TO DO
        //                 entry.Entity.Created = DateTime.UtcNow;
        //                 break;

        //             case EntityState.Modified:
        //                 //entry.Entity.LastModifiedBy = TO DO
        //                 entry.Entity.LastModified = DateTime.UtcNow;
        //                 break;
        //         }
        //     }

        //     var result = await base.SaveChangesAsync(cancellationToken);

        //     return result;
        // }
    }
}
