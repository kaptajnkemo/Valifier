using System.Reflection;

namespace Valifier.Application.Tests;

public sealed class ApplicationFeatureTests
{
    [Fact]
    public void Application_contains_dashboard_feature_slice()
    {
        var assembly = Assembly.Load("Valifier.Application");

        Assert.NotNull(assembly.GetType("Valifier.Application.Features.Dashboard.GetPlatformOverviewQuery"));
        Assert.NotNull(assembly.GetType("Valifier.Application.Features.Dashboard.GetPlatformOverviewQueryHandler"));
        Assert.NotNull(assembly.GetType("Valifier.Application.Features.Dashboard.PlatformOverview"));
    }
}
