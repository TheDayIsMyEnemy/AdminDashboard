using BlazorTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorTemplate.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
#pragma warning disable CS8618
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {


            base.OnModelCreating(builder);
        }
    }
}
