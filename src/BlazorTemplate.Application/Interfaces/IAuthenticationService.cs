using System.Security.Claims;

namespace ${repo_name}.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ClaimsPrincipal?> GetUser();
    }
}