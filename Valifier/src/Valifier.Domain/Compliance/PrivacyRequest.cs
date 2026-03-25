namespace Valifier.Domain.Compliance;

public sealed class PrivacyRequest : IGovernedRecord
{
    public PrivacyRequest(
        Guid id,
        string subjectIdentifier,
        PrivacyRequestType requestType,
        DateTimeOffset submittedTimestamp,
        PrivacyRequestStatus status)
    {
        if (string.IsNullOrWhiteSpace(subjectIdentifier))
        {
            throw new ArgumentException("Subject identifier is required.", nameof(subjectIdentifier));
        }

        Id = id;
        SubjectIdentifier = subjectIdentifier.Trim();
        RequestType = requestType;
        SubmittedTimestamp = submittedTimestamp;
        Status = status;
    }

    private PrivacyRequest()
    {
        Id = Guid.NewGuid();
        SubjectIdentifier = string.Empty;
        SubmittedTimestamp = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public string SubjectIdentifier { get; private set; }

    public PrivacyRequestType RequestType { get; private set; }

    public DateTimeOffset SubmittedTimestamp { get; private set; }

    public PrivacyRequestStatus Status { get; private set; }

    public void UpdateStatus(PrivacyRequestStatus status)
    {
        Status = status;
    }

    public string GetRecordIdentifier()
    {
        return Id.ToString("D");
    }

    public string GetRecordType()
    {
        return "PrivacyRequest";
    }

    public DateTimeOffset GetCollectionTimestamp()
    {
        return SubmittedTimestamp;
    }

    public string GetDataCategoryCode()
    {
        return "PRIVACY-REQUEST";
    }

    public string GetRetentionRuleCode()
    {
        return "PRIVACY-OPERATIONS";
    }

    public string GetSubjectScope()
    {
        return "User";
    }
}
