using BlazorTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorTemplate.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
#pragma warning disable CS8618
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Member> Members => Set<Member>();
        public DbSet<MemberActivityLog> MemberActivityLogs => Set<MemberActivityLog>();
        public DbSet<MemberIPConstraint> MemberIPConstraints => Set<MemberIPConstraint>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Member>().HasIndex(p => p.IdentityGuid).IsUnique(true);
            builder
                .Entity<Member>()
                .OwnsOne(
                    o => o.Address,
                    a =>
                    {
                        a.WithOwner();
                        a.Property(a => a.Street).HasMaxLength(180).IsRequired();
                        a.Property(a => a.City).HasMaxLength(100).IsRequired();
                        a.Property(a => a.State).HasMaxLength(60);
                        a.Property(a => a.Country).HasMaxLength(90).IsRequired();
                        a.Property(a => a.PostCode).HasMaxLength(20).IsRequired();
                    }
                );

            builder.Entity<Company>().HasIndex(c => c.CompanyName).IsUnique(true);
            builder
                .Entity<Company>()
                .OwnsOne(
                    o => o.Address,
                    a =>
                    {
                        a.WithOwner();
                        a.Property(a => a.Street).HasMaxLength(180).IsRequired();
                        a.Property(a => a.City).HasMaxLength(100).IsRequired();
                        a.Property(a => a.State).HasMaxLength(60);
                        a.Property(a => a.Country).HasMaxLength(90).IsRequired();
                        a.Property(a => a.PostCode).HasMaxLength(20).IsRequired();
                    }
                );

            base.OnModelCreating(builder);
        }
    }
}
