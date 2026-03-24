namespace Valifier.Application.Features.Dashboard;

public interface IPlatformOverviewReader
{
    Task<PlatformOverview> GetAsync(CancellationToken cancellationToken);
}
