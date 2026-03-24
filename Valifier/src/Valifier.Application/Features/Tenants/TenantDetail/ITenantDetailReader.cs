namespace Valifier.Application.Features.Tenants.TenantDetail;

public interface ITenantDetailReader
{
    Task<TenantDetailView?> GetAsync(Guid tenantId, CancellationToken cancellationToken);
}
