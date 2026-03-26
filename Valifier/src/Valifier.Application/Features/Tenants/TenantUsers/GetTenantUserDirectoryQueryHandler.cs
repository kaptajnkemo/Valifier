namespace Valifier.Application.Features.Tenants.TenantUsers;

public sealed class GetTenantUserDirectoryQueryHandler
{
    private readonly ITenantUserDirectoryReader _reader;

    public GetTenantUserDirectoryQueryHandler(ITenantUserDirectoryReader reader)
    {
        _reader = reader;
    }

    public Task<TenantUserDirectoryView?> HandleAsync(
        GetTenantUserDirectoryQuery query,
        CancellationToken cancellationToken = default)
    {
        return _reader.GetAsync(query.UserId, cancellationToken);
    }
}
