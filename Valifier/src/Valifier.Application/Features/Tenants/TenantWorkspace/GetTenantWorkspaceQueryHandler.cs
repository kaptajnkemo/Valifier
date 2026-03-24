namespace Valifier.Application.Features.Tenants.TenantWorkspace;

public sealed class GetTenantWorkspaceQueryHandler
{
    private readonly ITenantWorkspaceReader _reader;

    public GetTenantWorkspaceQueryHandler(ITenantWorkspaceReader reader)
    {
        _reader = reader;
    }

    public Task<TenantWorkspaceView?> HandleAsync(
        GetTenantWorkspaceQuery query,
        CancellationToken cancellationToken = default)
    {
        return _reader.GetAsync(query.UserId, cancellationToken);
    }
}
