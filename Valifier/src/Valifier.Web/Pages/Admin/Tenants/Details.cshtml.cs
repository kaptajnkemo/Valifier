using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valifier.Application.Features.Tenants.TenantDetail;
using Valifier.Domain.Identity;

namespace Valifier.Web.Pages.Admin.Tenants;

[Authorize(Roles = RoleNames.Admin)]
public sealed class DetailsModel : PageModel
{
    private readonly GetTenantDetailQueryHandler _handler;

    public DetailsModel(GetTenantDetailQueryHandler handler)
    {
        _handler = handler;
    }

    public TenantDetailView? Tenant { get; private set; }

    public async Task OnGetAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        Tenant = await _handler.HandleAsync(new GetTenantDetailQuery(tenantId), cancellationToken);
    }
}
