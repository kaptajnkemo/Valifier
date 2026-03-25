namespace Valifier.Application.Features.PrivacyRequests;

public interface IPrivacyRequestService
{
    Task<PrivacyRequestView> CreateAsync(CreatePrivacyRequestCommand command, CancellationToken cancellationToken);

    Task<PrivacyRequestView?> UpdateStatusAsync(UpdatePrivacyRequestStatusCommand command, CancellationToken cancellationToken);
}
