using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorTemplate.Application.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorTemplate.Server.Services
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