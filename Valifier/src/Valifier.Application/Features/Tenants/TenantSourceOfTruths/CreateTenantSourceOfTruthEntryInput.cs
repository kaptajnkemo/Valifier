namespace Valifier.Application.Features.Tenants.TenantSourceOfTruths;

public sealed record CreateTenantSourceOfTruthEntryInput(
    string Key,
    string Label,
    string ValueType,
    string Value);
