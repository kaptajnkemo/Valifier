namespace Valifier.Application.Features.Tenants.TenantSourceOfTruths;

public sealed record TenantSourceOfTruthDirectoryView(
    int TotalSourcesOfTruth,
    IReadOnlyList<TenantSourceOfTruthDirectoryRow> SourcesOfTruth);
