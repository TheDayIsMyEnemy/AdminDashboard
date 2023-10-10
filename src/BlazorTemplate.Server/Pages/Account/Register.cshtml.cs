// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using BlazorTemplate.Infrastructure.Identity;
using System.ComponentModel.DataAnnotations;

namespace BlazorTemplate.Server.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public CreateUserAccount Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class CreateUserAccount
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            //[DataType(DataType.Password)]
            //[Display(Name = "Confirm password")]
            //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            //public string ConfirmPassword { get; set; }

            public string FirstName { get; set; }
            public string LastName { get; set; }
        }


        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public IActionResult OnPostAsync(string returnUrl = null)
        {
            return Page();
            //returnUrl ??= Url.Content("~/");
            ////ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            //if (ModelState.IsValid)
            //{
            //    var user = new User { FirstName = Input.FirstName, LastName = Input.LastName };

            //    await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            //    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
            //    var result = await _userManager.CreateAsync(user, Input.Password);

            //    if (result.Succeeded)
            //    {
            //        _logger.LogInformation("User created a new account with password.");

            //        //var userId = await _userManager.GetUserIdAsync(user);
            //        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //        //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //        //var callbackUrl = Url.Page(
            //        //    "/Account/ConfirmEmail",
            //        //    pageHandler: null,
            //        //    values            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI BlazorTemplate.Infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
            //        //    protocol: Request.Scheme);

            //        //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
            //        //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            //        //if (_userManager.Options.SignIn.RequireConfirmedAccount)
            //        //{
            //        //    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
            //        //}
            //        //else
            //        //{
            //            await _signInManager.SignInAsync(user, isPersistent: false);
            //            return LocalRedirect(returnUrl);
            //        //}
            //    }
            //    foreach (var error in result.Errors)
            //    {
            //        ModelState.AddModelError(string.Empty, error.Description);
            //    }
            //}

            //// If we got this far, something failed, redisplay form
            //return Page();
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}
