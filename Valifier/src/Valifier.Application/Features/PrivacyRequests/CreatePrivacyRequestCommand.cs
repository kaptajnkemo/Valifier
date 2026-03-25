namespace Valifier.Application.Features.PrivacyRequests;

public sealed record CreatePrivacyRequestCommand(string SubjectIdentifier, string RequestType);
