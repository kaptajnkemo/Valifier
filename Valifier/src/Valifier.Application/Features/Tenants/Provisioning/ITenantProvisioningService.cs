namespace Valifier.Application.Features.Tenants.Provisioning;

public interface ITenantProvisioningService
{
    Task<Guid> CreateAsync(CreateTenantCommand command, CancellationToken cancellationToken);
}
