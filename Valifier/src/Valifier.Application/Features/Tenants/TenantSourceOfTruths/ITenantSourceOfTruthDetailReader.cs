namespace Valifier.Application.Features.Tenants.TenantSourceOfTruths;

public interface ITenantSourceOfTruthDetailReader
{
    Task<TenantSourceOfTruthDetailView?> GetAsync(Guid actorUserId, Guid sourceOfTruthId, CancellationToken cancellationToken);
}
