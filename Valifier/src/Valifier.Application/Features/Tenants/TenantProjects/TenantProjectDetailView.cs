namespace Valifier.Application.Features.Tenants.TenantProjects;

public sealed record TenantProjectDetailView(
    Guid ProjectId,
    string Title,
    string Department,
    string OwnerDisplayName,
    string SourceOfTruthName);
