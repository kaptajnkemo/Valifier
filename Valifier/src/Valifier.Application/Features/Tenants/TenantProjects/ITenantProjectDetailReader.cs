namespace Valifier.Application.Features.Tenants.TenantProjects;

public interface ITenantProjectDetailReader
{
    Task<TenantProjectDetailView?> GetAsync(Guid actorUserId, Guid projectId, CancellationToken cancellationToken);
}
