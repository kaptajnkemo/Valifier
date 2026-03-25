namespace Valifier.Application.Features.Tenants.TenantSourceOfTruths;

public sealed record TenantSourceOfTruthEntryView(
    string Key,
    string Label,
    string ValueType,
    string Value);
