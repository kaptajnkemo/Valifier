namespace Valifier.Application.Features.Dashboard;

public sealed class GetPlatformOverviewQueryHandler
{
    private readonly IPlatformOverviewReader _reader;

    public GetPlatformOverviewQueryHandler(IPlatformOverviewReader reader)
    {
        _reader = reader;
    }

    public Task<PlatformOverview> HandleAsync(
        GetPlatformOverviewQuery query,
        CancellationToken cancellationToken = default)
    {
        return _reader.GetAsync(cancellationToken);
    }
}
