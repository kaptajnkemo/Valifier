namespace Valifier.Application.Features.PrivacyRequests;

public sealed record UpdatePrivacyRequestStatusCommand(Guid RequestIdentifier, string Status);
