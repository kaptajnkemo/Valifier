namespace Valifier.Application.Features.Tenants.TenantSourceOfTruths;

public sealed record TenantSourceOfTruthDetailView(
    Guid SourceOfTruthId,
    string Topic,
    string Name,
    string SchemaVersion,
    IReadOnlyList<TenantSourceOfTruthEntryView> Entries);
