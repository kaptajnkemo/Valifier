namespace Valifier.Contracts;

public sealed class PrivacyRequestContract
{
    public required string RequestIdentifier { get; init; }

    public required string SubjectIdentifier { get; init; }

    public required string RequestType { get; init; }

    public required DateTimeOffset SubmittedTimestamp { get; init; }

    public required string Status { get; init; }
}
