namespace Valifier.Application.Features.PrivacyRequests;

public sealed class UpdatePrivacyRequestStatusCommandHandler
{
    private readonly IPrivacyRequestService _service;

    public UpdatePrivacyRequestStatusCommandHandler(IPrivacyRequestService service)
    {
        _service = service;
    }

    public Task<PrivacyRequestView?> HandleAsync(
        UpdatePrivacyRequestStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        return _service.UpdateStatusAsync(command, cancellationToken);
    }
}
