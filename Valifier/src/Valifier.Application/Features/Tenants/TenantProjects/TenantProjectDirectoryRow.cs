namespace Valifier.Application.Features.Tenants.TenantProjects;

public sealed record TenantProjectDirectoryRow(
    Guid ProjectId,
    string Title,
    string Department,
    string Status);
