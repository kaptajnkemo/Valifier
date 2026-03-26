namespace Valifier.Application.Features.Tenants.TenantSourceOfTruths;

public sealed class CreateTenantSourceOfTruthCommandHandler
{
    private readonly ITenantSourceOfTruthCreationService _service;

    public CreateTenantSourceOfTruthCommandHandler(ITenantSourceOfTruthCreationService service)
    {
        _service = service;
    }

    public Task<Guid> HandleAsync(CreateTenantSourceOfTruthCommand command, CancellationToken cancellationToken = default)
    {
        return _service.CreateAsync(command, cancellationToken);
    }
}
