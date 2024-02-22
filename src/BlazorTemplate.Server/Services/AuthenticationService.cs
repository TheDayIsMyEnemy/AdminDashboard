using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ${repo_name}.Application.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace ${repo_name}.Server.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthenticationService(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<ClaimsPrincipal?> GetUser()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

            return authState.User;
        }
    }
}