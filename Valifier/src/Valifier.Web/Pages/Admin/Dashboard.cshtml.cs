using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valifier.Application.Features.Tenants.AdminDashboard;
using Valifier.Domain.Identity;

namespace Valifier.Web.Pages.Admin;

[Authorize(Roles = RoleNames.Admin)]
public sealed class DashboardModel : PageModel
{
    private readonly GetAdminDashboardQueryHandler _handler;

    public DashboardModel(GetAdminDashboardQueryHandler handler)
    {
        _handler = handler;
    }

    public AdminDashboardView Dashboard { get; private set; } = new(0, []);

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Dashboard = await _handler.HandleAsync(new GetAdminDashboardQuery(), cancellationToken);
    }
}
