namespace Valifier.Application.Features.Tenants.TenantProjects;

public sealed record GetTenantProjectDetailQuery(Guid ActorUserId, Guid ProjectId);
