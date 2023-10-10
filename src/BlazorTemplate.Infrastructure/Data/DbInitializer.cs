using BlazorTemplate.Infrastructure.Data;
using BlazorTemplate.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlazorTemplate.Infrastructure
{
    public class DbInitializer
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<AppDbContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<Role>>();
            var logger = services.GetRequiredService<ILogger<DbInitializer>>();

            try
            {
                await context.Database.MigrateAsync();

                if (!roleManager.Roles.Any())
                {
                    foreach (var roleName in RoleType.List)
                    {
                        await roleManager.CreateAsync(new Role(roleName));
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("{ExceptionMessage}", ex.Message);
            }
        }

        private static async Task<User> CreateUser(
            string email,
            string password,
            string role,
            UserManager<User> userManager
        )
        {
            var user = new User
            {
                UserName = email,
                Email = email,
                IsDisabled = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            if (!await userManager.IsInRoleAsync(user, role))
                await userManager.AddToRoleAsync(user, role);

            return user;
        }
    }
}
