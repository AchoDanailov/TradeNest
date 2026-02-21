// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using static TradeNest.GCommon.EntityValidationConstants.ApplicationUser;
using TradeNest.Data.Models;

namespace TradeNest.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            this._userManager = userManager;
            this._userStore = userStore;
            this._emailStore = GetEmailStore();
            this._signInManager = signInManager;
            this._logger = logger;
            this._emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; } = null!;

        public string? ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
            = new List<AuthenticationScheme>();
        

        public class InputModel
        {
            [Required]
            [StringLength(UserNameMaxLengthValue, MinimumLength = UserNameMinLengthValue)]
            [Display(Name = "UserName")]
            public string UserName { get; set; } = null!;
            
            [Required]
            [EmailAddress]
            [StringLength(EmailMaxLengthValue, MinimumLength = EmailMinLengthValue)]
            [Display(Name = "Email")]
            public string Email { get; set; } = null!;

            [Required]
            [StringLength(PasswordMaxLengthValue, MinimumLength = PasswordMinLengthValue)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = null!;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = null!;
        }


        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Action("Index", controller: "Home");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .ToList();
            
            if (ModelState.IsValid)
            {
                ApplicationUser user = CreateUser();

                await this._userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
                await this._emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                IdentityResult result = await this._userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    this._logger.LogInformation("User created a new account with password.");
                    await this._signInManager.SignInAsync(user, isPersistent: true);
                    return LocalRedirect(returnUrl!);
                }
                
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return new ApplicationUser();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!this._userManager.SupportsUserEmail)
                throw new NotSupportedException("The default UI requires a user store with email support.");

            return (IUserEmailStore<ApplicationUser>)this._userStore;
        }
    }
}
