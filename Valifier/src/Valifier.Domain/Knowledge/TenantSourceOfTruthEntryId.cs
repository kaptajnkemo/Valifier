namespace Valifier.Domain.Knowledge;

public readonly record struct TenantSourceOfTruthEntryId(Guid Value)
{
    public static TenantSourceOfTruthEntryId New() => new(Guid.NewGuid());
}
