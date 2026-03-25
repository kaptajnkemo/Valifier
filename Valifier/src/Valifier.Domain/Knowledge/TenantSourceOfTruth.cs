using Valifier.Domain.Tenancy;

namespace Valifier.Domain.Knowledge;

public sealed class TenantSourceOfTruth
{
    private readonly List<TenantSourceOfTruthEntry> _entries;

    public TenantSourceOfTruth(
        TenantSourceOfTruthId id,
        TenantId tenantId,
        string topic,
        string name,
        string schemaVersion,
        IEnumerable<TenantSourceOfTruthEntry>? entries = null)
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
        _entries = entries?.ToList() ?? [];
    }

    private TenantSourceOfTruth()
    {
        Id = TenantSourceOfTruthId.New();
        TenantId = TenantId.New();
        Topic = string.Empty;
        Name = string.Empty;
        SchemaVersion = string.Empty;
        _entries = [];
    }

    public TenantSourceOfTruthId Id { get; private set; }

    public TenantId TenantId { get; private set; }

    public string Topic { get; private set; }

    public string Name { get; private set; }

    public string SchemaVersion { get; private set; }

    public IReadOnlyList<TenantSourceOfTruthEntry> Entries => _entries;

    public void AddEntry(TenantSourceOfTruthEntry entry)
    {
        _entries.Add(entry);
    }
}
