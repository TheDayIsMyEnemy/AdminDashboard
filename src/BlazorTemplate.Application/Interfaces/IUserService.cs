using BlazorTemplate.Domain.Common;
using BlazorTemplate.Infrastructure.Identity;

namespace BlazorTemplate.Application.Interfaces
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

        Task<ServiceResult> DeleteUser(
            string currentUserId,
            string userIdToDelete);

        Task<ServiceResult> AssignRoles(
            string currentUserId,
            string userIdToAssignRoles,
            IEnumerable<string> roles);

        Task<IEnumerable<string>> GetUserRoles(string userId);
    }
}