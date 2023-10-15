using System.ComponentModel.DataAnnotations;
using BlazorTemplate.Application.Interfaces;
using BlazorTemplate.UserInterface.Constants;
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

            var message = string.Format(ResultMessages.Created, FormModel.Email);
            var severity = Severity.Success;

            var result = await UserService.CreateUser(FormModel.Email, FormModel.Email, FormModel.Password, FormModel.FirstName, FormModel.LastName);

            if (!result.IsSuccess)
            {
                message = result.ToString();
                severity = Severity.Error;
            }

            IsLoading = false;
            Snackbar.Add(message, severity);
            MudDialog.Close();
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
