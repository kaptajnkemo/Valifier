namespace Valifier.Domain.Knowledge;

public readonly record struct TenantSourceOfTruthId(Guid Value)
{
    public static TenantSourceOfTruthId New() => new(Guid.NewGuid());
}
