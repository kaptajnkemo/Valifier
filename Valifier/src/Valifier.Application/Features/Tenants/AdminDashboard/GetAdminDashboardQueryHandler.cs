namespace Valifier.Application.Features.Tenants.AdminDashboard;

public sealed class GetAdminDashboardQueryHandler
{
    private readonly IAdminDashboardReader _reader;

    public GetAdminDashboardQueryHandler(IAdminDashboardReader reader)
    {
        _reader = reader;
    }

    public Task<AdminDashboardView> HandleAsync(
        GetAdminDashboardQuery query,
        CancellationToken cancellationToken = default)
    {
        return _reader.GetAsync(cancellationToken);
    }
}
