using Microsoft.EntityFrameworkCore;
using Valifier.Application.Features.Tenants.TenantDetail;
using Valifier.Domain.Tenancy;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Infrastructure.Services;

public sealed class TenantDetailReader : ITenantDetailReader
{
    private readonly ValifierDbContext _dbContext;

    public TenantDetailReader(ValifierDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<TenantDetailView?> GetAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var tenantIdentifier = new TenantId(tenantId);
        return _dbContext.Tenants
            .Where(tenant => tenant.Id == tenantIdentifier)
            .Select(tenant => new TenantDetailView(
                tenant.Id.Value,
                tenant.Name,
                tenant.InitialSuperuserDisplayName,
                tenant.InitialSuperuserEmail,
                tenant.InitialSuperuserHasSignedIn))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
