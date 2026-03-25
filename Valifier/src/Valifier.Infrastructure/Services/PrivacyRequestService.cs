using Microsoft.EntityFrameworkCore;
using Valifier.Application.Features.PrivacyRequests;
using Valifier.Domain.Compliance;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Infrastructure.Services;

public sealed class PrivacyRequestService : IPrivacyRequestService
{
    private readonly ValifierDbContext _dbContext;

    public PrivacyRequestService(ValifierDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PrivacyRequestView> CreateAsync(
        CreatePrivacyRequestCommand command,
        CancellationToken cancellationToken)
    {
        var subjectIdentifier = command.SubjectIdentifier?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(subjectIdentifier))
        {
            throw new ArgumentException("Subject identifier is required.", nameof(command.SubjectIdentifier));
        }

        var request = new PrivacyRequest(
            Guid.NewGuid(),
            subjectIdentifier,
            ParseRequestType(command.RequestType),
            DateTimeOffset.UtcNow,
            PrivacyRequestStatus.Received);

        _dbContext.PrivacyRequests.Add(request);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return PrivacyRequestMapping.ToView(request);
    }

    public async Task<PrivacyRequestView?> UpdateStatusAsync(
        UpdatePrivacyRequestStatusCommand command,
        CancellationToken cancellationToken)
    {
        var request = await _dbContext.PrivacyRequests
            .SingleOrDefaultAsync(candidate => candidate.Id == command.RequestIdentifier, cancellationToken);

        if (request is null)
        {
            return null;
        }

        request.UpdateStatus(ParseStatus(command.Status));
        await _dbContext.SaveChangesAsync(cancellationToken);

        return PrivacyRequestMapping.ToView(request);
    }

    private static PrivacyRequestType ParseRequestType(string requestType)
    {
        if (Enum.TryParse<PrivacyRequestType>(requestType, ignoreCase: true, out var parsed))
        {
            return parsed;
        }

        throw new ArgumentException("Request type is invalid.", nameof(requestType));
    }

    private static PrivacyRequestStatus ParseStatus(string status)
    {
        if (Enum.TryParse<PrivacyRequestStatus>(status, ignoreCase: true, out var parsed))
        {
            return parsed;
        }

        throw new ArgumentException("Status is invalid.", nameof(status));
    }
}
