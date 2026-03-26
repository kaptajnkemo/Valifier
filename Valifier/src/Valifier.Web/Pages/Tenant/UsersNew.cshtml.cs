using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valifier.Application.Features.Tenants.TenantUsers;
using Valifier.Domain.Identity;
using Valifier.Infrastructure.Identity;

namespace Valifier.Web.Pages.Tenant;

[Authorize(Roles = RoleNames.Superuser)]
public sealed class UsersNewModel : PageModel
{
    private readonly CreateHiringManagerCommandHandler _handler;
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersNewModel(
        CreateHiringManagerCommandHandler handler,
        UserManager<ApplicationUser> userManager)
    {
        _handler = handler;
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

        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Redirect("/sign-in");
        }

        await _handler.HandleAsync(
            new CreateHiringManagerCommand(
                user.Id,
                Input.DisplayName,
                Input.Email,
                Input.Password),
            cancellationToken);

        return Redirect("/tenant/users");
    }

    public sealed class InputModel
    {
        [Required]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
