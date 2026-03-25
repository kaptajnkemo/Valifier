namespace Valifier.Application.Features.Tenants.TenantSourceOfTruths;

public sealed record GetTenantSourceOfTruthDetailQuery(Guid ActorUserId, Guid SourceOfTruthId);
