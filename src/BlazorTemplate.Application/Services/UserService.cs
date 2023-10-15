using System.Security.Claims;
using BlazorTemplate.Application.Interfaces;
using BlazorTemplate.Domain.Common;
using BlazorTemplate.Domain.Constants;
using BlazorTemplate.Domain.Entities;
using BlazorTemplate.Domain.Extensions;
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
        private readonly RoleManager<Role> _roleManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IDbContextFactory<AppDbContext> appDbContextFactory,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IAuthenticationService authenticationService,
            ILogger<UserService> logger)
        {
            _appDbContextFactory = appDbContextFactory;
            _userManager = userManager;
            _roleManager = roleManager;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        public async Task<ServiceResult> DeleteUser(string userId)
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null)
                throw new ArgumentNullException("Current user not found");

            if (currentUser.Id.Equals(userId))
                throw new ArgumentException("Invalid delete action");

            var userToDelete = await _userManager.FindByIdAsync(userId);
            if (userToDelete == null)
                throw new ArgumentNullException("User to delete not found");

            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            var isCurrentUserSuperAdmin = currentUserRoles.Any(r => r.Equals(Roles.SuperAdmin));
            var isCurrentUserAdmin = currentUserRoles.Any(r => r.Equals(Roles.Admin));
            if (!isCurrentUserSuperAdmin && !isCurrentUserAdmin)
                return ServiceResult.Error("You do not have permission to perform this action.");

            var userToDeleteRoles = await _userManager.GetRolesAsync(userToDelete);
            var isUserToDeleteSuperAdmin = userToDeleteRoles.Any(r => r.Equals(Roles.SuperAdmin));
            var isUserToDeleteAdmin = currentUserRoles.Any(r => r.Equals(Roles.Admin));
            if (isUserToDeleteSuperAdmin)
                return ServiceResult.Error($"{Roles.SuperAdmin} cannot be deleted");
            if (isUserToDeleteAdmin && !isCurrentUserSuperAdmin)
                throw new InvalidOperationException($"Only {Roles.SuperAdmin} can perform this action");

            using var appDbContext = _appDbContextFactory.CreateDbContext();
            var member = await appDbContext.Members.FirstOrDefaultAsync(m => m.IdentityGuid == userToDelete.Id);
            if (member == null)
                throw new ArgumentNullException("Member not found");

            var identityResult = await _userManager.DeleteAsync(userToDelete);
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

        public async Task<ServiceResult> AssignRoles(
            string userId,
            IEnumerable<string> newRoles)
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null)
                throw new ArgumentNullException("Current user not found");

            var userToModify = await _userManager.FindByIdAsync(userId);
            if (userToModify == null)
                throw new ArgumentNullException("User to assign roles not found");

            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            var isCurrentUserSuperAdmin = currentUserRoles.Any(r => r.Equals(Roles.SuperAdmin));
            var isCurrentUserAdmin = currentUserRoles.Any(r => r.Equals(Roles.Admin));

            if (!isCurrentUserSuperAdmin && !isCurrentUserAdmin)
                return ServiceResult.Error("Only admin users can assign roles");

            var userToModifyRoles = await _userManager.GetRolesAsync(userToModify);
            var isUserToModifySuperAdmin = userToModifyRoles.Any(r => r.Equals(Roles.SuperAdmin));
            var isUserToModifyAdmin = userToModifyRoles.Any(r => r.Equals(Roles.Admin));

            if (isUserToModifySuperAdmin || (isCurrentUserAdmin && isUserToModifyAdmin))
                return ServiceResult.Error("No permission to perform this action.");

            var rolesToAdd = newRoles.Except(userToModifyRoles).ToList();
            var rolesToRemove = userToModifyRoles.Except(newRoles).ToList();
            var hasRolesToAdd = rolesToAdd.Any();
            var hasRolesToRemove = rolesToRemove.Any();

            if (!hasRolesToAdd && !hasRolesToRemove)
                return ServiceResult.Error("There are no roles to modify");

            if (hasRolesToRemove)
            {
                var identityResult = await _userManager.RemoveFromRolesAsync(userToModify, rolesToRemove);
                if (!identityResult.Succeeded)
                    return ServiceResult.Error(identityResult.Errors.Select(e => e.Description).ToArray());
            }
            if (hasRolesToAdd)
            {
                var identityResult = await _userManager.AddToRolesAsync(userToModify, rolesToAdd);
                if (!identityResult.Succeeded)
                    return ServiceResult.Error(identityResult.Errors.Select(e => e.Description).ToArray());
            }

            return ServiceResult.Success;
        }

        public async Task<IEnumerable<string>> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ArgumentNullException("User not found");

            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }

        public async Task<IEnumerable<string>> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles.Select(r => r.Name)!;
        }

        public async Task<ServiceResult> SetUserAccountStatus(
            string userId,
            UserAccountStatus accountStatus)
        {
            var user = GetCurrentUser();
            if (user == null)
                throw new ArgumentNullException("Current user not found");

            if (user.Id!.Equals(userId))
                throw new ArgumentException("No permission");

            return ServiceResult.Success;
        }

        private async Task<User?> GetCurrentUser()
        {
            var claimsPrincipal = await _authenticationService.GetUser();
            if (claimsPrincipal == null)
                return null;

            return await _userManager.FindByIdAsync(claimsPrincipal.GetId()!);
        }
    }
}