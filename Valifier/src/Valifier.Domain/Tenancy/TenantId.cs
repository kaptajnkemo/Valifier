namespace Valifier.Domain.Tenancy;

public readonly record struct TenantId(Guid Value)
{
    public static TenantId New()
    {
        return new TenantId(Guid.NewGuid());
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
