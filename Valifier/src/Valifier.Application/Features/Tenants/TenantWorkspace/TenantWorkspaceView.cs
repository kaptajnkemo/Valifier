namespace Valifier.Application.Features.Tenants.TenantWorkspace;

public sealed record TenantWorkspaceView(
    string TenantName,
    string SignedInUserEmail,
    int TotalTenantUsers,
    int TotalTenantSourcesOfTruth,
    int TotalTenantProjects);
