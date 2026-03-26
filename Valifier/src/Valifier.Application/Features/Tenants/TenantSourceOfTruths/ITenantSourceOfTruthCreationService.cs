namespace Valifier.Application.Features.Tenants.TenantSourceOfTruths;

public interface ITenantSourceOfTruthCreationService
{
    Task<Guid> CreateAsync(CreateTenantSourceOfTruthCommand command, CancellationToken cancellationToken);
}
