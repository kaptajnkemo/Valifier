namespace Valifier.Application.Features.PrivacyRequests;

public sealed class GetPrivacyRequestQueryHandler
{
    private readonly IPrivacyRequestReader _reader;

    public GetPrivacyRequestQueryHandler(IPrivacyRequestReader reader)
    {
        _reader = reader;
    }

    public Task<PrivacyRequestView?> HandleAsync(
        GetPrivacyRequestQuery query,
        CancellationToken cancellationToken = default)
    {
        return _reader.GetAsync(query.RequestIdentifier, cancellationToken);
    }
}
