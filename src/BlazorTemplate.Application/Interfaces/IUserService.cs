using BlazorTemplate.Domain.Models;
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
    }
}