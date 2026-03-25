namespace Valifier.Domain.Compliance;

public interface IGovernedRecord
{
    string GetRecordIdentifier();

    string GetRecordType();

    DateTimeOffset GetCollectionTimestamp();

    string GetDataCategoryCode();

    string GetRetentionRuleCode();

    string GetSubjectScope();
}
