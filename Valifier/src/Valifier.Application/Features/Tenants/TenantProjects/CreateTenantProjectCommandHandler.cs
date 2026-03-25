namespace Valifier.Application.Features.Tenants.TenantProjects;

public sealed class CreateTenantProjectCommandHandler
{
    private readonly ITenantProjectCreationService _service;

    public CreateTenantProjectCommandHandler(ITenantProjectCreationService service)
    {
        _service = service;
    }

    public Task<Guid> HandleAsync(CreateTenantProjectCommand command, CancellationToken cancellationToken = default)
    {
        return _service.CreateAsync(command, cancellationToken);
    }
}
