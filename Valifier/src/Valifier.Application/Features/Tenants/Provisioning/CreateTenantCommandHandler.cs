namespace Valifier.Application.Features.Tenants.Provisioning;

public sealed class CreateTenantCommandHandler
{
    private readonly ITenantProvisioningService _service;

    public CreateTenantCommandHandler(ITenantProvisioningService service)
    {
        _service = service;
    }

    public Task<Guid> HandleAsync(
        CreateTenantCommand command,
        CancellationToken cancellationToken = default)
    {
        return _service.CreateAsync(command, cancellationToken);
    }
}
