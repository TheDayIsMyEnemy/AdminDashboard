using System.Globalization;
using FilterShop.Domain;
using FilterShop.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using FilterShop.UI.Components;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorTemplate.UI.Components;

namespace FilterShop.UI.Pages.UserManagement
{
    public class IndexBase : ComponentBase
    {
        protected bool IsLoading { get; set; }
        protected string? SearchQuery { get; set; }
        protected string CurrentUserId { get; set; } = null!;
        protected List<User> Users { get; set; } = new();

        [Inject]
        protected UserManager<User> UserManager { get; set; } = null!;

        [Inject]
        protected ISnackbar Snackbar { get; set; } = null!;

        [Inject]
        protected IDialogService DialogService { get; set; } = null!;

        // [Inject]
        // protected IRepository<Member> MemberRepository { get; set; } = null!;

        [Inject]
        protected ICompanyService CompanyService { get; set; } = null!;

        [CascadingParameter]
        protected Task<AuthenticationState> AuthStateTask { get; set; } = null!;

        private async Task LoadUsers()
        {
            IsLoading = true;

            Users = await UserManager.Users.ToListAsync();

            var authState = await AuthStateTask;
            CurrentUserId = authState.User.GetId()!;

            IsLoading = false;
        }

        protected override async Task OnInitializedAsync() => await LoadUsers();

        protected bool FilterUsers(User user)
        {
            if (string.IsNullOrEmpty(SearchQuery))
                return true;
            else if (user.Email.Includes(SearchQuery))
                return true;
            else if (user.PhoneNumber.Includes(SearchQuery))
                return true;
            else if (user.IpAddress.Includes(SearchQuery))
                return true;
            else if (user.UserAgent.Includes(SearchQuery))
                return true;

            return false;
        }

        protected async Task AssignRoles(User user)
        {
            // var authState = await AuthStateTask;
            // var currentUser = await UserManager.GetUserAsync(authState.User);
            // var currentUserRoles = await UserManager.GetRolesAsync(currentUser!);
            // if (!currentUserRoles.Any(r => r == RoleNames.SuperAdmin))
            // {
            //     Snackbar.Add(
            //         "You cannot modify roles for this user",
            //         Severity.Warning
            //     );
            // }

            var currentRoles = await UserManager.GetRolesAsync(user);
            // if (currentRoles.Any(r => r == RoleNames.SuperAdmin || r == RoleNames.Admin)) { }

            var parameters = new DialogParameters
            {
                { "UserRoles", currentRoles },
                { "SelectMultipleRoles", true }
            };

            var result = await DialogService.Show<AssignRole>("Assign Roles", parameters).Result;

            if (!result.Canceled)
            {
                IdentityResult? identityResult = null;
                var newRoles = (IEnumerable<string>)result.Data;
                var rolesToAdd = newRoles.Except(currentRoles).ToList();
                var rolesToRemove = currentRoles.Except(newRoles).ToList();

                if (rolesToRemove.Any())
                {
                    identityResult = await UserManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!identityResult.Succeeded)
                    {
                        Snackbar.Add(
                            string.Join(
                                Environment.NewLine,
                                identityResult.Errors.Select(e => e.Description)
                            ),
                            Severity.Error
                        );
                        return;
                    }
                }
                if (rolesToAdd.Any())
                {
                    identityResult = await UserManager.AddToRolesAsync(user, rolesToAdd);
                    if (!identityResult.Succeeded)
                    {
                        Snackbar.Add(
                            string.Join(
                                Environment.NewLine,
                                identityResult.Errors.Select(e => e.Description)
                            ),
                            Severity.Error
                        );
                        return;
                    }
                }
                if (identityResult != null)
                {
                    Snackbar.Add(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            ResultMessages.Updated,
                            user.Email
                        ),
                        Severity.Success
                    );
                }
            }
        }

        public async Task OpenCreateUserDialog()
        {
            var result = await DialogService.Show<Create>("Create user").Result;
            if (!result.Canceled)
            {
                await LoadUsers();
                StateHasChanged();
            }
        }

        protected async Task EnableOrDisableUserAccount(User user)
        {
            var userRoles = await UserManager.GetRolesAsync(user);
            if (userRoles.Any(r => r == RoleNames.SuperAdmin))
            {
                Snackbar.Add(ResultMessages.NoPermissionToPerformThisAction, Severity.Warning);
                return;
            }

            var action = user.IsEnabled ? "Disable" : "Enable";

            user.IsEnabled = !user.IsEnabled;
            var identityResult = await UserManager.UpdateAsync(user);

            if (identityResult.Succeeded)
            {
                Snackbar.Add($"User account {user.Email} has been {action}d.", Severity.Success);
            }
            else
            {
                Snackbar.Add(
                    string.Join(
                        Environment.NewLine,
                        identityResult.Errors.Select(e => e.Description)
                    ),
                    Severity.Error
                );
            }
        }

        protected async Task ResetAccountLockout(User user)
        {
            var parameters = new DialogParameters
            {
                { "Title", "Reset account lockout" },
                { "TitleIcon", Icons.Material.Filled.LockReset },
                { "TitleIconColor", Color.Warning },
                {
                    "Content",
                    $"Are you sure you want to reset account lockout for \"{user.Email}\"?"
                },
                { "ConfirmationButtonText", "Reset" }
            };

            var result = await DialogService.Show<ConfirmationModal>("", parameters).Result;

            if (!result.Canceled)
            {
                user.LockoutEnd = null;
                var identityResult = await UserManager.UpdateAsync(user);

                if (identityResult.Succeeded)
                {
                    Snackbar.Add(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            ResultMessages.Updated,
                            user.Email
                        ),
                        Severity.Success
                    );
                }
                else
                {
                    Snackbar.Add(
                        string.Join(
                            Environment.NewLine,
                            identityResult.Errors.Select(e => e.Description)
                        ),
                        Severity.Error
                    );
                }
            }
        }

        protected async Task DeleteUser(User user)
        {
            var userRoles = await UserManager.GetRolesAsync(user);
            if (userRoles.Any(r => r == RoleNames.SuperAdmin))
            {
                Snackbar.Add(ResultMessages.NoPermissionToPerformThisAction, Severity.Warning);
                return;
            }

            var parameters = new DialogParameters
            {
                { "Title", "Delete user" },
                { "TitleIcon", Icons.Material.Filled.DeleteForever },
                { "TitleIconColor", Color.Error },
                { "Content", $"Are you sure you want to delete \"{user.Email}\"?" },
                { "ConfirmationButtonText", "Delete" },
                { "ConfirmationButtonColor", Color.Error },
            };

            var result = await DialogService.Show<ConfirmationModal>("", parameters).Result;

            if (!result.Canceled)
            {
                var identityResult = await UserManager.DeleteAsync(user);
                if (identityResult.Succeeded)
                {
                    Users.Remove(user);

                    // var member = await MemberRepository.FirstOrDefault(
                    //     m => m.AccountId == user.Id
                    // );

                    // if (member != null)
                    // {
                    //     await OrganizationService.RemoveMember(
                    //         member!.OrganizationId,
                    //         member.AccountId
                    //     );
                    // }

                    Snackbar.Add(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            ResultMessages.Deleted,
                            user.Email
                        ),
                        Severity.Success
                    );
                }
                else
                {
                    Snackbar.Add(
                        string.Join(
                            Environment.NewLine,
                            identityResult.Errors.Select(e => e.Description)
                        ),
                        Severity.Error
                    );
                }
            }
        }
    }
}
