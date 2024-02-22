using BlazorTemplate.Application.Interfaces;
using BlazorTemplate.Domain.Extensions;
using BlazorTemplate.Infrastructure.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BlazorTemplate.Server.Services
{
    public class AuthStateProvider :
        RevalidatingServerAuthenticationStateProvider
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IdentityOptions _options;
        // private readonly NavigationManager _nav;
        // private readonly IUserService _userService;

        public AuthStateProvider(
            ILoggerFactory loggerFactory,
            IServiceScopeFactory scopeFactory,
            IOptions<IdentityOptions> optionsAccessor)
            : base(loggerFactory)
        {
            _scopeFactory = scopeFactory;
            _options = optionsAccessor.Value;
            // _nav = nav;
        }

        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(1);

        public async Task<ClaimsPrincipal> GetAuthenticationState()
        {
            var authState = await base.GetAuthenticationStateAsync();
            return authState.User;
        }

        protected override async Task<bool> ValidateAuthenticationStateAsync(
            AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            // Get the user manager from a new scope to ensure it fetches fresh data
            var scope = _scopeFactory.CreateScope();
            try
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var isValid = await ValidateSecurityStampAsync(userManager, authenticationState.User);

                return isValid;
            }
            finally
            {
                if (scope is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else
                {
                    scope.Dispose();
                }
            }
        }

        private async Task<bool> ValidateSecurityStampAsync(UserManager<User> userManager, ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal);
            if (user == null || user.AccountStatus == UserAccountStatus.Disabled)
            {
                // _nav.NavigateTo(Constants.LogoutPath, true);
                return false;
            }
            else if (!userManager.SupportsUserSecurityStamp)
            {
                return true;
            }
            else
            {
                var principalStamp = principal.FindFirstValue(_options.ClaimsIdentity.SecurityStampClaimType);
                var userStamp = await userManager.GetSecurityStampAsync(user!);
                return principalStamp == userStamp;
            }
        }
    }
}
