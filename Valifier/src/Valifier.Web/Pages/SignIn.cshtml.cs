using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valifier.Application.Features.Tenants.SignInTracking;
using Valifier.Domain.Identity;
using Valifier.Infrastructure.Identity;

namespace Valifier.Web.Pages;

public sealed class SignInModel : PageModel
{
    private readonly IInitialTenantSignInRecorder _initialTenantSignInRecorder;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public SignInModel(
        IInitialTenantSignInRecorder initialTenantSignInRecorder,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _initialTenantSignInRecorder = initialTenantSignInRecorder;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(
            Input.Email.Trim(),
            Input.Password,
            isPersistent: false,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid sign-in attempt.");
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Input.Email.Trim());

        if (user is null)
        {
            await _signInManager.SignOutAsync();
            ModelState.AddModelError(string.Empty, "Invalid sign-in attempt.");
            return Page();
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains(RoleNames.Admin, StringComparer.Ordinal))
        {
            return Redirect("/admin/dashboard");
        }

        if (roles.Contains(RoleNames.Superuser, StringComparer.Ordinal) && user.TenantId.HasValue)
        {
            await _initialTenantSignInRecorder.RecordAsync(user.Id, cancellationToken);
            return Redirect("/tenant/dashboard");
        }

        await _signInManager.SignOutAsync();
        ModelState.AddModelError(string.Empty, "This account does not have an allowed route.");
        return Page();
    }

    public sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
