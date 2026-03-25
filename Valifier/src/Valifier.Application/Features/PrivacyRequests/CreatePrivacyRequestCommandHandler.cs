namespace Valifier.Application.Features.PrivacyRequests;

public sealed class CreatePrivacyRequestCommandHandler
{
    private readonly IPrivacyRequestService _service;

    public CreatePrivacyRequestCommandHandler(IPrivacyRequestService service)
    {
        _service = service;
    }

    public Task<PrivacyRequestView> HandleAsync(
        CreatePrivacyRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        return _service.CreateAsync(command, cancellationToken);
    }
}
