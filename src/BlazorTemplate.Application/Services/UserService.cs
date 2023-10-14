using BlazorTemplate.Application.Interfaces;
using BlazorTemplate.Domain;
using BlazorTemplate.Domain.Models;
using BlazorTemplate.Infrastructure.Data;
using BlazorTemplate.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlazorTemplate.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IDbContextFactory<AppDbContext> _appDbContextFactory;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IDbContextFactory<AppDbContext> appDbContextFactory,
            UserManager<User> userManager,
            ILogger<UserService> logger)
        {
            _appDbContextFactory = appDbContextFactory;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ServiceResult> DeleteUser(
            string currentUserId,
            string userIdToDelete)
        {
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            if (currentUser == null)
                throw new ArgumentNullException("Current user not found");

            var userToDelete = await _userManager.FindByIdAsync(userIdToDelete);
            if (userToDelete == null)
                throw new ArgumentNullException("User to delete not found");

            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            var isCurrentUserSuperAdmin = currentUserRoles.Any(r => r.Equals(Roles.SuperAdmin));
            var isCurrentUserAdmin = currentUserRoles.Any(r => r.Equals(Roles.Admin));
            if (!isCurrentUserSuperAdmin && !isCurrentUserAdmin)
                return ServiceResult.Error("Current user has no permissions");

            var userToDeleteRoles = await _userManager.GetRolesAsync(userToDelete);
            var isUserToDeleteSuperAdmin = currentUserRoles.Any(r => r.Equals(Roles.SuperAdmin));
            var isUserToDeleteAdmin = currentUserRoles.Any(r => r.Equals(Roles.Admin));
            if (isUserToDeleteSuperAdmin)
                return ServiceResult.Error($"{Roles.SuperAdmin} cannot be deleted");
            if (isUserToDeleteAdmin && !isCurrentUserSuperAdmin)
                throw new InvalidOperationException($"Only {Roles.SuperAdmin} can perform this action");

            using var appDbContext = _appDbContextFactory.CreateDbContext();
            var member = await appDbContext.Members.FirstOrDefaultAsync(m => m.IdentityGuid == currentUserId);
            if (member == null)
                throw new ArgumentNullException("Member not found");

            var identityResult = await _userManager.DeleteAsync(currentUser);
            if (!identityResult.Succeeded)
                return ServiceResult.Error(identityResult.Errors.Select(e => e.Description).ToArray());

            appDbContext.Members.Remove(member);
            await appDbContext.SaveChangesAsync();

            _logger.LogInformation("{CurrentUserEmail} deleted user: {UserEmailToDelete}", currentUser.Email, userToDelete.Email);

            return ServiceResult.Success;
        }

        public async Task<ServiceResult> CreateUser(
            string userName,
            string email,
            string password,
            string firstName,
            string lastName)
        {
            var memberAccount = new User { UserName = userName, Email = email };
            var identityResult = await _userManager.CreateAsync(memberAccount, password);
            if (!identityResult.Succeeded)
            {
                ServiceResult.Error(identityResult.Errors.Select(e => e.Description).ToArray());
            }

            using var appDbContext = _appDbContextFactory.CreateDbContext();
            var member = new Member(firstName, lastName, memberAccount.Id);
            await appDbContext.Members.AddAsync(member);
            await appDbContext.SaveChangesAsync();

            _logger.LogInformation("New user created: {UserEmail}", email);

            return ServiceResult.Success;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
            => await _userManager.Users.ToListAsync();
    }
}