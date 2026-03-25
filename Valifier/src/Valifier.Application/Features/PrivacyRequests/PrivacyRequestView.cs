namespace Valifier.Application.Features.PrivacyRequests;

public sealed record PrivacyRequestView(
    Guid RequestIdentifier,
    string SubjectIdentifier,
    string RequestType,
    DateTimeOffset SubmittedTimestamp,
    string Status);
