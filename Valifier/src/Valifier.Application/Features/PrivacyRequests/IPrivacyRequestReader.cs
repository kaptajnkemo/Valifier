namespace Valifier.Application.Features.PrivacyRequests;

public interface IPrivacyRequestReader
{
    Task<PrivacyRequestView?> GetAsync(Guid requestIdentifier, CancellationToken cancellationToken);
}
