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
            {
                throw new ArgumentNullException(ResultMessages.Formats.NotFound, "CurrentUser");
            }

            if (currentUser.Id.Equals(userId))
            {
                return ServiceResult.Error.WithMessage(ResultMessages.InvalidAction);
            }

            var userToDelete = await _userManager.FindByIdAsync(userId);
            if (userToDelete == null)
            {
                throw new ArgumentNullException(ResultMessages.Formats.NotFound, userId);
            }

            var isCurrentUserAdmin = await _userManager.IsInRoleAsync(currentUser, Roles.Admin);
            var isUserToDeleteAdmin = await _userManager.IsInRoleAsync(userToDelete, Roles.Admin);
            if (!isCurrentUserAdmin || isUserToDeleteAdmin)
            {
                return ServiceResult.Error.WithMessage(ResultMessages.NoPermissionToPerformThisAction);
            }

            using var appDbContext = _appDbContextFactory.CreateDbContext();
            var member = await appDbContext.Members.FirstOrDefaultAsync(m => m.IdentityGuid == userToDelete.Id);
            if (member == null)
            {
                throw new ArgumentNullException($"Member with identity: {userToDelete.Id} not found");
            }

            var identityResult = await _userManager.DeleteAsync(userToDelete);
            if (!identityResult.Succeeded)
            {
                return ServiceResult.Error.WithMessage(identityResult.Errors.Select(e => e.Description).ToArray());
            }

            appDbContext.Members.Remove(member);
            await appDbContext.SaveChangesAsync();

            _logger.LogInformation(string.Format(ResultMessages.Formats.EntityPerformedActionOnEntity, currentUser.Email, "deleted", userToDelete.Email));

            return ServiceResult.Success.WithFormatMessage(ResultMessages.Formats.Deleted, userToDelete.Email!);
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
                return ServiceResult.Error.WithMessage(identityResult.Errors.Select(e => e.Description).ToArray());
            }

            using var appDbContext = _appDbContextFactory.CreateDbContext();
            var member = new Member(firstName, lastName, memberAccount.Id);
            await appDbContext.Members.AddAsync(member);
            await appDbContext.SaveChangesAsync();

            _logger.LogInformation(ResultMessages.Formats.Created, email);

            return ServiceResult.Success.WithFormatMessage(ResultMessages.Formats.Created, email);
        }

        public async Task<IEnumerable<User>> GetAllUsers()
            => await _userManager.Users.ToListAsync();

        public async Task<ServiceResult> AssignRoles(
            string userId,
            IEnumerable<string> newRoles)
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null)
            {
                throw new ArgumentNullException("Current user not found");
            }

            var isCurrentUserAdmin = await _userManager.IsInRoleAsync(currentUser, Roles.Admin);

            if (!isCurrentUserAdmin)
            {
                return ServiceResult.Error.WithMessage(ResultMessages.NoPermissionToPerformThisAction);
            }

            User? userToModify = null;

            if (currentUser.Id.Equals(userId))
            {
                userToModify = currentUser;
            }
            else
            {
                userToModify = await _userManager.FindByIdAsync(userId);
                if (userToModify == null)
                {
                    throw new ArgumentNullException(ResultMessages.Formats.NotFound, userId);
                }
                var isUserToModifyAdmin = await _userManager.IsInRoleAsync(userToModify, Roles.Admin);
                if (isUserToModifyAdmin)
                {
                    return ServiceResult.Error.WithMessage(ResultMessages.NoPermissionToPerformThisAction);
                }
            }

            var userToModifyRoles = await _userManager.GetRolesAsync(userToModify);

            var rolesToAdd = newRoles.Except(userToModifyRoles).ToList();
            var rolesToRemove = userToModifyRoles.Except(newRoles).ToList();
            var hasRolesToAdd = rolesToAdd.Any();
            var hasRolesToRemove = rolesToRemove.Any();

            if (!hasRolesToAdd && !hasRolesToRemove)
                return ServiceResult.Error.WithMessage(ResultMessages.InvalidAction);

            if (hasRolesToRemove)
            {
                var identityResult = await _userManager.RemoveFromRolesAsync(userToModify, rolesToRemove);
                if (!identityResult.Succeeded)
                    return ServiceResult.Error.WithMessage(identityResult.Errors.Select(e => e.Description).ToArray());
            }
            if (hasRolesToAdd)
            {
                var identityResult = await _userManager.AddToRolesAsync(userToModify, rolesToAdd);
                if (!identityResult.Succeeded)
                    return ServiceResult.Error.WithMessage(identityResult.Errors.Select(e => e.Description).ToArray());
            }

            _logger.LogInformation(string.Format(ResultMessages.Formats.EntityPerformedActionOnEntity, currentUser.Email, "updated roles for", userToModify.Email));

            return ServiceResult.Success.WithMessage(ResultMessages.Formats.Updated, userToModify.Email!);
        }

        public async Task<IEnumerable<string>> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentNullException(ResultMessages.Formats.NotFound, userId);
            }

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
            UserAccountStatus newAccountStatus)
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null)
            {
                throw new ArgumentNullException(ResultMessages.Formats.NotFound, "CurrentUser");
            }

            var isCurrentUserAdmin = await _userManager.IsInRoleAsync(currentUser, Roles.Admin);
            if (!isCurrentUserAdmin)
            {
                return ServiceResult.Error.WithMessage(ResultMessages.NoPermissionToPerformThisAction);
            }

            if (currentUser.Id!.Equals(userId))
            {
                return ServiceResult.Error.WithMessage(ResultMessages.InvalidAction);
            }

            var userToModify = await _userManager.FindByIdAsync(userId);
            if (userToModify == null)
            {
                throw new ArgumentNullException(ResultMessages.Formats.NotFound, userId);
            }
            if (userToModify.AccountStatus == newAccountStatus)
            {
                return ServiceResult.Error.WithMessage(ResultMessages.InvalidAction);
            }

            var isUserToModifyAdmin = await _userManager.IsInRoleAsync(userToModify, Roles.Admin);
            if (isUserToModifyAdmin && newAccountStatus == UserAccountStatus.Disabled)
            {
                return ServiceResult.Error.WithMessage(ResultMessages.NoPermissionToPerformThisAction);
            }

            userToModify.AccountStatus = newAccountStatus;
            await _userManager.UpdateAsync(userToModify);

            _logger.LogInformation(string.Format(ResultMessages.Formats.EntityPerformedActionOnEntity, currentUser.Id, $"updated account status to {newAccountStatus} for", userToModify.Email));

            return ServiceResult.Success.WithMessage($"{userToModify.Email}  account status is {newAccountStatus}");
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