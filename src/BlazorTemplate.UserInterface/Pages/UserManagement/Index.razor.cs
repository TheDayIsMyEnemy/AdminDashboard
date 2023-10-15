using MudBlazor;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorTemplate.Application.Interfaces;
using BlazorTemplate.Domain.Extensions;
using BlazorTemplate.Infrastructure.Identity;
using BlazorTemplate.UserInterface.Constants;
using BlazorTemplate.UserInterface.Components;

namespace BlazorTemplate.UserInterface.Pages.UserManagement
{
    public class IndexBase : ComponentBase
    {
        protected bool IsLoading { get; set; }
        protected string? SearchQuery { get; set; }
        protected string CurrentUserId { get; set; } = null!;
        protected IEnumerable<User> Users { get; set; } = Enumerable.Empty<User>();

        [Inject]
        protected IUserService UserService { get; set; } = null!;

        [Inject]
        protected ISnackbar Snackbar { get; set; } = null!;

        [Inject]
        protected IDialogService DialogService { get; set; } = null!;

        [CascadingParameter]
        protected Task<AuthenticationState> AuthStateTask { get; set; } = null!;

        private async Task LoadUsers()
        {
            IsLoading = true;
            Users = await UserService.GetAllUsers();
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

            return false;
        }

        protected async Task AssignRoles(User user)
        {
            // var parameters = new DialogParameters
            // {
            //     { "UserRoles", currentRoles },
            //     { "SelectMultipleRoles", true }
            // };

            // var dialogResult = await DialogService.Show<AssignRole>("Assign Roles", parameters).Result;

            // if (!dialogResult.Canceled)
            // {

            // }
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
            // var userRoles = await UserManager.GetRolesAsync(user);
            // if (userRoles.Any(r => r == Roles.Admin))
            // {
            //     Snackbar.Add(ResultMessages.NoPermissionToPerformThisAction, Severity.Warning);
            //     return;
            // }

            // var action = user.IsDisabled ? "Enable" : "Disable";

            // user.IsDisabled = !user.IsDisabled;
            // var identityResult = await UserManager.UpdateAsync(user);

            // if (identityResult.Succeeded)
            // {
            //     Snackbar.Add($"User account {user.Email} has been {action}d.", Severity.Success);
            // }
            // else
            // {
            //     Snackbar.Add(
            //         string.Join(
            //             Environment.NewLine,
            //             identityResult.Errors.Select(e => e.Description)
            //         ),
            //         Severity.Error
            //     );
            // }
        }

        protected async Task ResetAccountLockout(User user)
        {
            // var parameters = new DialogParameters
            // {
            //     { "Title", "Reset account lockout" },
            //     { "TitleIcon", Icons.Material.Filled.LockReset },
            //     { "TitleIconColor", Color.Warning },
            //     {
            //         "Content",
            //         $"Are you sure you want to reset account lockout for \"{user.Email}\"?"
            //     },
            //     { "ConfirmationButtonText", "Reset" }
            // };

            // var result = await DialogService.Show<ConfirmationModal>("", parameters).Result;

            // if (!result.Canceled)
            // {
            //     user.LockoutEnd = null;
            //     var identityResult = await UserManager.UpdateAsync(user);

            //     if (identityResult.Succeeded)
            //     {
            //         Snackbar.Add(
            //             string.Format(
            //                 CultureInfo.CurrentCulture,
            //                 ResultMessages.Updated,
            //                 user.Email
            //             ),
            //             Severity.Success
            //         );
            //     }
            //     else
            //     {
            //         Snackbar.Add(
            //             string.Join(
            //                 Environment.NewLine,
            //                 identityResult.Errors.Select(e => e.Description)
            //             ),
            //             Severity.Error
            //         );
            //     }
            // }
        }

        protected async Task DeleteUser(User user)
        {
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
                var deleteUserResult = await UserService.DeleteUser(CurrentUserId, user.Id);
                if (deleteUserResult.IsSuccess)
                {
                    Snackbar.Add(string.Format(CultureInfo.CurrentCulture, ResultMessages.Deleted, user.Email), Severity.Success);
                    await LoadUsers();
                }
                else
                {
                    Snackbar.Add(result.ToString(), Severity.Error);
                }
            }
        }
    }
}
