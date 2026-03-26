namespace Valifier.Application.Features.Tenants.TenantProjects;

public interface ITenantProjectCreationService
{
    Task<Guid> CreateAsync(CreateTenantProjectCommand command, CancellationToken cancellationToken);
}
