namespace Valifier.Application.Features.Tenants.TenantSourceOfTruths;

public sealed class GetTenantSourceOfTruthDetailQueryHandler
{
    private readonly ITenantSourceOfTruthDetailReader _reader;

    public GetTenantSourceOfTruthDetailQueryHandler(ITenantSourceOfTruthDetailReader reader)
    {
        _reader = reader;
    }

    public Task<TenantSourceOfTruthDetailView?> HandleAsync(
        GetTenantSourceOfTruthDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        return _reader.GetAsync(query.ActorUserId, query.SourceOfTruthId, cancellationToken);
    }
}
