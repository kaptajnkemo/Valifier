using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valifier.Application.Features.Tenants.TenantWorkspace;
using Valifier.Domain.Identity;
using Valifier.Infrastructure.Identity;

namespace Valifier.Web.Pages.Tenant;

[Authorize(Roles = RoleNames.Superuser)]
public sealed class DashboardModel : PageModel
{
    private readonly GetTenantWorkspaceQueryHandler _handler;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardModel(
        GetTenantWorkspaceQueryHandler handler,
        UserManager<ApplicationUser> userManager)
    {
        _handler = handler;
        _userManager = userManager;
    }

    public TenantWorkspaceView? Workspace { get; private set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Redirect("/sign-in");
        }

        Workspace = await _handler.HandleAsync(new GetTenantWorkspaceQuery(user.Id), cancellationToken);

        return Workspace is null
            ? Redirect("/sign-in")
            : Page();
    }
}
