namespace Valifier.Application.Features.Tenants.TenantSourceOfTruths;

public sealed record TenantSourceOfTruthDirectoryRow(
    Guid SourceOfTruthId,
    string Topic,
    string Name,
    string SchemaVersion);
