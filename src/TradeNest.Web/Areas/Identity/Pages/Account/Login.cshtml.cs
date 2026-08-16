// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

using TradeNest.Data.Models;
using static TradeNest.GCommon.EntityValidationConstants.User;

namespace TradeNest.Web.Areas.Identity.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(SignInManager<ApplicationUser> signInManager,
        ILogger<LoginModel> logger, UserManager<ApplicationUser> userManager)
    {
        this._signInManager = signInManager;
        this._logger = logger;
        this._userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public IList<AuthenticationScheme> ExternalLogins { get; set; }
        = new List<AuthenticationScheme>();

    public string? ReturnUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(UserNameOrEmailMaxLengthValue, MinimumLength = UserNameOrEmailMinLengthValue)]
        [Display(Name = nameof(UserNameOrEmail))]
        public string UserNameOrEmail { get; set; } = null!;
        
        [Required]
        [DataType(DataType.Password)]
        [StringLength(PasswordMaxLengthValue, MinimumLength = PasswordMinLengthValue)]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
            ModelState.AddModelError(string.Empty, ErrorMessage);

        returnUrl ??= Url.Action("Index", controller: "Home");

        if (this._signInManager.IsSignedIn(User))
            return LocalRedirect(returnUrl!);

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
            .ToList();

        ReturnUrl = returnUrl;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Action("Index", controller: "Home");

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
            .ToList();

        if (ModelState.IsValid)
        {
            SignInResult userNameSignInResult = await this._signInManager.PasswordSignInAsync(
                Input.UserNameOrEmail, Input.Password, Input.RememberMe, lockoutOnFailure: true);
            if (userNameSignInResult.Succeeded)
            {
                return LocalRedirect(returnUrl!);
            }
            else if (userNameSignInResult.IsLockedOut)
            {
                this._logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }
            else
            {
                ApplicationUser? user = await this._userManager
                    .FindByEmailAsync(Input.UserNameOrEmail);
                if (user != null)
                {
                    SignInResult emailSignInResult = await this._signInManager
                        .CheckPasswordSignInAsync(user, Input.Password, true);
                    if (emailSignInResult.Succeeded)
                    {
                        await this._signInManager.SignInAsync(user, isPersistent: true);
                        return LocalRedirect(returnUrl!);
                    }
                    else if (emailSignInResult.IsLockedOut)
                    {
                        this._logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                }
                
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
        }

        // If we got this far, something failed, redisplay form
        return Page();
    }
}