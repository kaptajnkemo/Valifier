using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valifier.Application.Features.Tenants.TenantSourceOfTruths;
using Valifier.Domain.Identity;
using Valifier.Infrastructure.Identity;

namespace Valifier.Web.Pages.Tenant;

[Authorize(Roles = RoleNames.Superuser)]
public sealed class SourceOfTruthDetailsModel : PageModel
{
    private readonly GetTenantSourceOfTruthDetailQueryHandler _handler;
    private readonly UserManager<ApplicationUser> _userManager;

    public SourceOfTruthDetailsModel(
        GetTenantSourceOfTruthDetailQueryHandler handler,
        UserManager<ApplicationUser> userManager)
    {
        _handler = handler;
        _userManager = userManager;
    }

    public TenantSourceOfTruthDetailView? SourceOfTruth { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid sotId, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Redirect("/sign-in");
        }

        SourceOfTruth = await _handler.HandleAsync(new GetTenantSourceOfTruthDetailQuery(user.Id, sotId), cancellationToken);
        return Page();
    }
}
