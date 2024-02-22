using MudBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorTemplate.Application.Interfaces;
using BlazorTemplate.Domain.Extensions;
using BlazorTemplate.Infrastructure.Identity;
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

        public async Task AssignRoles(string userIdToAssignRoles)
        {
            var parameters = new DialogParameters
            {
                { "UserIdToAssignRoles", userIdToAssignRoles },
                { "SelectMultipleRoles", true }
            };

            var dialogResult = await DialogService.Show<AssignRole>("Assign Roles", parameters).Result;

            if (!dialogResult.Canceled)
            {
                await LoadUsers();
                StateHasChanged();
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

        protected async Task SetAccountStatus(User user)
        {
            var newAccountStatus = user.AccountStatus == UserAccountStatus.Enabled ? UserAccountStatus.Disabled : UserAccountStatus.Enabled;

            var serviceResult = await UserService.SetUserAccountStatus(user.Id, newAccountStatus);

            Snackbar.Add(serviceResult.ToString(), serviceResult.IsSuccess ? Severity.Success : Severity.Error);
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

            var dialogResult = await DialogService.Show<ConfirmationModal>("", parameters).Result;

            if (!dialogResult.Canceled)
            {
                var serviceResult = await UserService.DeleteUser(user.Id);
                var resultMessage = serviceResult.ToString();

                if (serviceResult.IsSuccess)
                {
                    Snackbar.Add(resultMessage, Severity.Success);
                    await LoadUsers();
                }
                else
                {
                    Snackbar.Add(resultMessage, Severity.Error);
                }
            }
        }
    }
}
