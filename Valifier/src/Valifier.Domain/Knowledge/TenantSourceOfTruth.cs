using Valifier.Domain.Tenancy;

namespace Valifier.Domain.Knowledge;

public sealed class TenantSourceOfTruth
{
    public TenantSourceOfTruth(
        TenantSourceOfTruthId id,
        TenantId tenantId,
        string topic,
        string name,
        string schemaVersion)
    {
        if (string.IsNullOrWhiteSpace(topic))
        {
            throw new ArgumentException("Topic is required.", nameof(topic));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(schemaVersion))
        {
            throw new ArgumentException("Schema version is required.", nameof(schemaVersion));
        }

        Id = id;
        TenantId = tenantId;
        Topic = topic.Trim();
        Name = name.Trim();
        SchemaVersion = schemaVersion.Trim();
    }

    private TenantSourceOfTruth()
    {
        Id = TenantSourceOfTruthId.New();
        TenantId = TenantId.New();
        Topic = string.Empty;
        Name = string.Empty;
        SchemaVersion = string.Empty;
    }

    public TenantSourceOfTruthId Id { get; private set; }

    public TenantId TenantId { get; private set; }

    public string Topic { get; private set; }

    public string Name { get; private set; }

    public string SchemaVersion { get; private set; }
}
