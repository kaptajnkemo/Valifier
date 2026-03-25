using Valifier.Domain.Compliance;
using Valifier.Domain.Identity;

namespace Valifier.Domain.Tenancy;

public sealed class Tenant : IGovernedRecord
{
    public Tenant(
        TenantId id,
        string name,
        string initialSuperuserDisplayName,
        string initialSuperuserEmail,
        UserId initialSuperuserUserId,
        bool initialSuperuserHasSignedIn = false,
        DateTimeOffset? collectedAtUtc = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tenant name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(initialSuperuserDisplayName))
        {
            throw new ArgumentException("Initial superuser display name is required.", nameof(initialSuperuserDisplayName));
        }

        if (string.IsNullOrWhiteSpace(initialSuperuserEmail))
        {
            throw new ArgumentException("Initial superuser email is required.", nameof(initialSuperuserEmail));
        }

        Id = id;
        Name = name.Trim();
        InitialSuperuserDisplayName = initialSuperuserDisplayName.Trim();
        InitialSuperuserEmail = initialSuperuserEmail.Trim();
        InitialSuperuserUserId = initialSuperuserUserId;
        InitialSuperuserHasSignedIn = initialSuperuserHasSignedIn;
        CollectedAtUtc = collectedAtUtc ?? DateTimeOffset.UtcNow;
    }

    private Tenant()
    {
        Id = TenantId.New();
        Name = string.Empty;
        InitialSuperuserDisplayName = string.Empty;
        InitialSuperuserEmail = string.Empty;
        InitialSuperuserUserId = new UserId(Guid.Empty);
        CollectedAtUtc = DateTimeOffset.UtcNow;
    }

    public TenantId Id { get; private set; }

    public string Name { get; private set; }

    public string InitialSuperuserDisplayName { get; private set; }

    public string InitialSuperuserEmail { get; private set; }

    public UserId InitialSuperuserUserId { get; private set; }

    public bool InitialSuperuserHasSignedIn { get; private set; }

    public DateTimeOffset CollectedAtUtc { get; private set; }

    public void MarkInitialSuperuserSignedIn()
    {
        InitialSuperuserHasSignedIn = true;
    }

    public string GetRecordIdentifier()
    {
        return Id.Value.ToString("D");
    }

    public string GetRecordType()
    {
        return "Tenant";
    }

    public DateTimeOffset GetCollectionTimestamp()
    {
        return CollectedAtUtc;
    }

    public string GetDataCategoryCode()
    {
        return "TENANT-PROFILE";
    }

    public string GetRetentionRuleCode()
    {
        return "TENANT-ACCOUNT";
    }

    public string GetSubjectScope()
    {
        return "Tenant";
    }
}
