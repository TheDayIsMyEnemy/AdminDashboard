using BlazorTemplate.Domain.Models;
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
            base.OnModelCreating(builder);
        }
    }
}
