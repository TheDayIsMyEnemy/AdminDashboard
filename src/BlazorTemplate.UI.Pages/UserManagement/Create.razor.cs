using System.ComponentModel.DataAnnotations;
using FilterShop.Domain;
using FilterShop.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor;

namespace FilterShop.UI.Pages.UserManagement
{
    public class CreateBase : ComponentBase
    {
        protected bool IsLoading { get; set; }
        protected CreateUserFormModel FormModel { get; } = new();
        protected bool IsAdminUser { get; set; }
        protected IEnumerable<Domain.Company> Companies { get; set; } =
            Enumerable.Empty<Domain.Company>();

        [Inject]
        protected ICompanyService CompanyService { get; set; } = null!;

        // [Inject]
        // protected IRepository<Member> MemberRepository { get; set; } = null!;

        [Inject]
        protected UserManager<User> UserManager { get; set; } = null!;

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

            var user = new User
            {
                Email = FormModel.Email,
                UserName = FormModel.Email,
                FirstName = FormModel.FirstName,
                LastName = FormModel.LastName,
                CompanyId = FormModel.OrganizationId
            };

            var result = await UserManager.CreateAsync(user, FormModel.Password!);

            if (result.Succeeded)
            {
                // if (FormModel.OrganizationId.HasValue)
                // {
                //     await OrganizationService.AddMember(
                //         FormModel.OrganizationId.Value,
                //         FormModel.FirstName!,
                //         FormModel.LastName!,
                //         user.Id
                //     );
                // }
            }
            else
            {
                message = string.Join(
                    Environment.NewLine,
                    result.Errors.Select(e => e.Description)
                );
                severity = Severity.Error;
            }

            Snackbar.Add(message, severity);

            IsLoading = false;

            MudDialog.Close();
        }

        protected override async Task OnParametersSetAsync()
        {
            var authState = await AuthStateTask;
            IsAdminUser =
                authState.User.IsInRole(RoleNames.SuperAdmin)
                || authState.User.IsInRole(RoleNames.Admin);

            if (IsAdminUser)
            {
                Companies = await CompanyService.GetAllCompanies();
            }
            else
            {
                // FormModel.OrganizationId = (
                //     await MemberRepository.FirstOrDefault(
                //         m => m.AccountId == authState.User.GetId()!
                //     )
                // )!.OrganizationId;
            }
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
        public string? Password { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        public int? OrganizationId { get; set; }
    }
}
