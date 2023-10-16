using System.ComponentModel.DataAnnotations;
using BlazorTemplate.Application.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace BlazorTemplate.UserInterface.Pages.UserManagement
{
    public class CreateBase : ComponentBase
    {
        protected bool IsLoading { get; set; }
        protected CreateUserFormModel FormModel { get; } = new();

        [Inject]
        protected IUserService UserService { get; set; } = null!;

        [Inject]
        protected ISnackbar Snackbar { get; set; } = null!;

        [CascadingParameter]
        protected MudDialogInstance MudDialog { get; set; } = null!;

        [CascadingParameter]
        protected Task<AuthenticationState> AuthStateTask { get; set; } = null!;

        protected async Task OnValidSubmit()
        {
            IsLoading = true;

            var serviceResult = await UserService.CreateUser(FormModel.Email, FormModel.Email, FormModel.Password, FormModel.FirstName, FormModel.LastName);

            if (serviceResult.IsSuccess)
            {
                Snackbar.Add(serviceResult.ToString(), Severity.Success);
                MudDialog.Close();
            }
            else
            {
                Snackbar.Add(serviceResult.ToString(), Severity.Error);
            }

            IsLoading = false;
        }
    }

    public class CreateUserFormModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(
            100,
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6
        )]

        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;
    }
}
