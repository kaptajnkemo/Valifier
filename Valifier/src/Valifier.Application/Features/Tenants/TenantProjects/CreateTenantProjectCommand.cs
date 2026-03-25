namespace Valifier.Application.Features.Tenants.TenantProjects;

public sealed record CreateTenantProjectCommand(
    Guid ActorUserId,
    string JobTitle,
    string Department,
    Guid SourceOfTruthId,
    Guid? OwnerUserId);
