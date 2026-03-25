namespace Valifier.Application.Features.Tenants.TenantUsers;

public sealed class CreateHiringManagerCommandHandler
{
    private readonly ITenantUserCreationService _service;

    public CreateHiringManagerCommandHandler(ITenantUserCreationService service)
    {
        _service = service;
    }

    public Task<Guid> HandleAsync(CreateHiringManagerCommand command, CancellationToken cancellationToken = default)
    {
        return _service.CreateHiringManagerAsync(command, cancellationToken);
    }
}
