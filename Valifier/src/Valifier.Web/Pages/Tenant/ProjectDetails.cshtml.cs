using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valifier.Application.Features.Tenants.TenantProjects;
using Valifier.Domain.Identity;
using Valifier.Infrastructure.Identity;

namespace Valifier.Web.Pages.Tenant;

[Authorize(Roles = RoleNames.TenantWorkspaceRoles)]
public sealed class ProjectDetailsModel : PageModel
{
    private readonly GetTenantProjectDetailQueryHandler _handler;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProjectDetailsModel(
        GetTenantProjectDetailQueryHandler handler,
        UserManager<ApplicationUser> userManager)
    {
        _handler = handler;
        _userManager = userManager;
    }

    public TenantProjectDetailView? Project { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid projectId, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Redirect("/sign-in");
        }

        Project = await _handler.HandleAsync(new GetTenantProjectDetailQuery(user.Id, projectId), cancellationToken);
        return Page();
    }
}
