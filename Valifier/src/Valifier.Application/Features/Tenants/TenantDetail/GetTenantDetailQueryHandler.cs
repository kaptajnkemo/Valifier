namespace Valifier.Application.Features.Tenants.TenantDetail;

public sealed class GetTenantDetailQueryHandler
{
    private readonly ITenantDetailReader _reader;

    public GetTenantDetailQueryHandler(ITenantDetailReader reader)
    {
        _reader = reader;
    }

    public Task<TenantDetailView?> HandleAsync(
        GetTenantDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        return _reader.GetAsync(query.TenantId, cancellationToken);
    }
}
