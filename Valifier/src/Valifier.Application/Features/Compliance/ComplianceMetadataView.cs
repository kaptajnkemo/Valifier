namespace Valifier.Application.Features.Compliance;

public sealed record ComplianceMetadataView(
    string RecordIdentifier,
    string DataCategoryCode,
    string RetentionRuleCode,
    DateTimeOffset CollectionTimestamp,
    string SubjectScope);
