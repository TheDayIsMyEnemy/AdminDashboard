using System.Security.Claims;
using ${repo_name}.Domain.Common;
using ${repo_name}.Infrastructure.Identity;

namespace ${repo_name}.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsers();

        Task<ServiceResult> CreateUser(
            string userName,
            string email,
            string password,
            string firstName,
            string lastName);

        Task<ServiceResult> DeleteUser(string userId);

        Task<ServiceResult> AssignRoles(
            string userId,
            IEnumerable<string> newRoles);

        Task<IEnumerable<string>> GetUserRoles(string userId);

        Task<IEnumerable<string>> GetAllRoles();

        Task<ServiceResult> SetUserAccountStatus(string userId, UserAccountStatus accountStatus);
    }
}