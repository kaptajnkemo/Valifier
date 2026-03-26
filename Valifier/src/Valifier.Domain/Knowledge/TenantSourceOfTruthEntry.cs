namespace Valifier.Domain.Knowledge;

public sealed class TenantSourceOfTruthEntry
{
    public TenantSourceOfTruthEntry(
        TenantSourceOfTruthEntryId id,
        TenantSourceOfTruthId sourceOfTruthId,
        string key,
        string label,
        string valueType,
        string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Entry key is required.", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(label))
        {
            throw new ArgumentException("Entry label is required.", nameof(label));
        }

        if (string.IsNullOrWhiteSpace(valueType))
        {
            throw new ArgumentException("Entry value type is required.", nameof(valueType));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Entry value is required.", nameof(value));
        }

        Id = id;
        SourceOfTruthId = sourceOfTruthId;
        Key = key.Trim();
        Label = label.Trim();
        ValueType = valueType.Trim();
        Value = value.Trim();
    }

    private TenantSourceOfTruthEntry()
    {
        Id = TenantSourceOfTruthEntryId.New();
        SourceOfTruthId = TenantSourceOfTruthId.New();
        Key = string.Empty;
        Label = string.Empty;
        ValueType = string.Empty;
        Value = string.Empty;
    }

    public TenantSourceOfTruthEntryId Id { get; private set; }

    public TenantSourceOfTruthId SourceOfTruthId { get; private set; }

    public string Key { get; private set; }

    public string Label { get; private set; }

    public string ValueType { get; private set; }

    public string Value { get; private set; }
}
