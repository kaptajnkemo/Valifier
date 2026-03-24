using Microsoft.EntityFrameworkCore;
using Valifier.Application.Features.Tenants.AdminDashboard;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Infrastructure.Services;

public sealed class AdminDashboardReader : IAdminDashboardReader
{
    private readonly ValifierDbContext _dbContext;

    public AdminDashboardReader(ValifierDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AdminDashboardView> GetAsync(CancellationToken cancellationToken)
    {
        var tenants = await _dbContext.Tenants
            .OrderBy(tenant => tenant.Name)
            .Select(tenant => new AdminDashboardTenantRow(
                tenant.Id.Value,
                tenant.Name,
                tenant.InitialSuperuserEmail,
                tenant.InitialSuperuserHasSignedIn))
            .ToArrayAsync(cancellationToken);

        return new AdminDashboardView(tenants.Length, tenants);
    }
}
