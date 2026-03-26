namespace Valifier.Application.Features.Tenants.TenantProjects;

public sealed class GetTenantProjectDetailQueryHandler
{
    private readonly ITenantProjectDetailReader _reader;

    public GetTenantProjectDetailQueryHandler(ITenantProjectDetailReader reader)
    {
        _reader = reader;
    }

    public Task<TenantProjectDetailView?> HandleAsync(
        GetTenantProjectDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        return _reader.GetAsync(query.ActorUserId, query.ProjectId, cancellationToken);
    }
}
