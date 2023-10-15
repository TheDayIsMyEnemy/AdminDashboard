using System.Security.Claims;

namespace BlazorTemplate.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ClaimsPrincipal?> GetUser();
    }
}