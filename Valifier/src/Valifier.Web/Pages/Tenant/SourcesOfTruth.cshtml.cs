using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valifier.Application.Features.Tenants.TenantSourceOfTruths;
using Valifier.Domain.Identity;
using Valifier.Infrastructure.Identity;

namespace Valifier.Web.Pages.Tenant;

[Authorize(Roles = RoleNames.Superuser)]
public sealed class SourcesOfTruthModel : PageModel
{
    private readonly GetTenantSourceOfTruthDirectoryQueryHandler _handler;
    private readonly UserManager<ApplicationUser> _userManager;

    public SourcesOfTruthModel(
        GetTenantSourceOfTruthDirectoryQueryHandler handler,
        UserManager<ApplicationUser> userManager)
    {
        _handler = handler;
        _userManager = userManager;
    }

    public TenantSourceOfTruthDirectoryView Directory { get; private set; } = new(0, []);

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Redirect("/sign-in");
        }

        var directory = await _handler.HandleAsync(new GetTenantSourceOfTruthDirectoryQuery(user.Id), cancellationToken);

        if (directory is null)
        {
            return Redirect("/sign-in");
        }

        Directory = directory;
        return Page();
    }
}
