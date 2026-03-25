namespace Valifier.Application.Features.Tenants.TenantProjects;

public sealed record TenantProjectDirectoryView(
    int TotalProjects,
    IReadOnlyList<TenantProjectDirectoryRow> Projects);
