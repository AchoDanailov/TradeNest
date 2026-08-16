// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using TradeNest.Data.Models;
using static TradeNest.GCommon.EntityValidationConstants.User;

namespace TradeNest.Web.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        SignInManager<ApplicationUser> signInManager,
        ILogger<RegisterModel> logger)
    {
        this._userManager = userManager;
        this._userStore = userStore;
        this._emailStore = GetEmailStore();
        this._signInManager = signInManager;
        this._logger = logger;
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

    
    public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Action("Index", controller: "Home");

        if (this._signInManager.IsSignedIn(User))
            return LocalRedirect(ReturnUrl!);
            
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Action("Index", controller: "Home");

        if (this._signInManager.IsSignedIn(User))
            return LocalRedirect(returnUrl!);
        
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
            .ToList();
            
        if (ModelState.IsValid)
        {
            ApplicationUser user = new ApplicationUser();

            await this._userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
            await this._emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
            IdentityResult result = await this._userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                if(this._signInManager.IsSignedIn(User))
                    await this._signInManager.SignOutAsync();
                
                await this._signInManager.SignInAsync(user, isPersistent: true);
                return LocalRedirect(returnUrl!);
            }
                
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }

        // If we got this far, something failed, redisplay form
        return Page();
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!this._userManager.SupportsUserEmail)
            throw new NotSupportedException("The default UI requires a user store with email support.");

        return (IUserEmailStore<ApplicationUser>)this._userStore;
    }
}