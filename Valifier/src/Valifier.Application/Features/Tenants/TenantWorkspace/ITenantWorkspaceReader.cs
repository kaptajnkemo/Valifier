namespace Valifier.Application.Features.Tenants.TenantWorkspace;

public interface ITenantWorkspaceReader
{
    Task<TenantWorkspaceView?> GetAsync(Guid userId, CancellationToken cancellationToken);
}
