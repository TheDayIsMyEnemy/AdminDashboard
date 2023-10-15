using System.Net;
using BlazorTemplate.Application.Services;
using BlazorTemplate.Application.Interfaces;
using BlazorTemplate.Infrastructure.Data;
using BlazorTemplate.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static BlazorTemplate.Infrastructure.Identity.Constants;
using Microsoft.AspNetCore.Components.Server.Circuits;
using BlazorTemplate.Server.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

namespace BlazorTemplate.Server
{
    public static class AppServicesConfig
    {
        public static void AddSecurity(this IServiceCollection services)
        {
            services
                .AddIdentity<User, Role>(options =>
                {
                    options.Stores.MaxLengthForKeys = 128;
                    options.SignIn.RequireConfirmedAccount = true;
                    options.User.RequireUniqueEmail = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(24);
                    options.Lockout.MaxFailedAccessAttempts = 3;
                    //options.User.AllowedUserNameCharacters = null;
                })
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddUserConfirmation<UserConfirmation>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(5);
                options.SlidingExpiration = true;
                options.LoginPath = new PathString(LoginPath);
                options.LogoutPath = new PathString(LogoutPath);
            });

            // services.ConfigurePolicy();
        }

        public static void AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment
        )
        {
            services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
        }

        public static void AddHttpClients(
            this IServiceCollection services,
            IConfiguration config)
        {

        }

        public static void ConfigureOptions(
            this IServiceCollection services,
            ConfigurationManager config
        )
        {

        }
    }
}
