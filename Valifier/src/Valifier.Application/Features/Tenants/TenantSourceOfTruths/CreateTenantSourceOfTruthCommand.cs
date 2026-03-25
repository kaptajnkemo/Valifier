namespace Valifier.Application.Features.Tenants.TenantSourceOfTruths;

public sealed record CreateTenantSourceOfTruthCommand(
    Guid ActorUserId,
    string Topic,
    string Name,
    string SchemaVersion,
    IReadOnlyList<CreateTenantSourceOfTruthEntryInput> Entries);
