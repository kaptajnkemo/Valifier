namespace Valifier.Contracts;

public sealed class ComplianceMetadataContract
{
    public required string RecordIdentifier { get; init; }

    public required string DataCategoryCode { get; init; }

    public required string RetentionRuleCode { get; init; }

    public required DateTimeOffset CollectionTimestamp { get; init; }

    public required string SubjectScope { get; init; }
}
