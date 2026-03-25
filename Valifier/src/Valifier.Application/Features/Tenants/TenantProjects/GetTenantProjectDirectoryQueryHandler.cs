namespace Valifier.Application.Features.Tenants.TenantProjects;

public sealed class GetTenantProjectDirectoryQueryHandler
{
    private readonly ITenantProjectDirectoryReader _reader;

    public GetTenantProjectDirectoryQueryHandler(ITenantProjectDirectoryReader reader)
    {
        _reader = reader;
    }

    public Task<TenantProjectDirectoryView?> HandleAsync(
        GetTenantProjectDirectoryQuery query,
        CancellationToken cancellationToken = default)
    {
        return _reader.GetAsync(query.UserId, cancellationToken);
    }
}
