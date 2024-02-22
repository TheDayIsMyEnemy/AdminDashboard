using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BlazorTemplate.Domain.Constants;

namespace BlazorTemplate.Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task Seed(
            AppIdentityDbContext appIdentityDbContext,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            ILogger logger)
        {

            if (appIdentityDbContext.Database.IsMySql())
                await appIdentityDbContext.Database.MigrateAsync();

            if (!roleManager.Roles.Any())
            {
                foreach (var roleName in Roles.List)
                {
                    await roleManager.CreateAsync(new Role(roleName));
                }
                logger.LogInformation("Roles created");
            }

            if (!userManager.Users.Any())
            {
                await CreateUser(
                    "admin@blazor.template",
                    "Password11!",
                    Roles.Admin,
                    userManager
                );
                logger.LogInformation("Users created");
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