namespace Valifier.Application.Features.Tenants.TenantSourceOfTruths;

public sealed class GetTenantSourceOfTruthDirectoryQueryHandler
{
    private readonly ITenantSourceOfTruthDirectoryReader _reader;

    public GetTenantSourceOfTruthDirectoryQueryHandler(ITenantSourceOfTruthDirectoryReader reader)
    {
        _reader = reader;
    }

    public Task<TenantSourceOfTruthDirectoryView?> HandleAsync(
        GetTenantSourceOfTruthDirectoryQuery query,
        CancellationToken cancellationToken = default)
    {
        return _reader.GetAsync(query.UserId, cancellationToken);
    }
}
