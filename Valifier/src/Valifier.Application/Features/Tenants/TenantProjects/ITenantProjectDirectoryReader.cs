namespace Valifier.Application.Features.Tenants.TenantProjects;

public interface ITenantProjectDirectoryReader
{
    Task<TenantProjectDirectoryView?> GetAsync(Guid userId, CancellationToken cancellationToken);
}
