using Valifier.Application.Features.PrivacyRequests;
using Valifier.Domain.Compliance;

namespace Valifier.Infrastructure.Services;

internal static class PrivacyRequestMapping
{
    public static PrivacyRequestView ToView(PrivacyRequest request)
    {
        return new PrivacyRequestView(
            request.Id,
            request.SubjectIdentifier,
            request.RequestType.ToString(),
            request.SubmittedTimestamp,
            request.Status.ToString());
    }
}
