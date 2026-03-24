namespace Valifier.Application.Features.Dashboard;

public sealed record PlatformOverview(
    string PlatformName,
    string Summary,
    IReadOnlyList<PlatformMetric> Metrics,
    IReadOnlyList<PlatformModuleOverview> Modules,
    IReadOnlyList<PlatformRoleOverview> Roles);

public sealed record PlatformMetric(string Label, string Value, string Detail);

public sealed record PlatformModuleOverview(string Name, string Purpose, string Stage, string Route);

public sealed record PlatformRoleOverview(string Name, string Responsibility);
