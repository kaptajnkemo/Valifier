using Microsoft.EntityFrameworkCore;
using Valifier.Application.Features.PrivacyRequests;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Infrastructure.Services;

public sealed class PrivacyRequestReader : IPrivacyRequestReader
{
    private readonly ValifierDbContext _dbContext;

    public PrivacyRequestReader(ValifierDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PrivacyRequestView?> GetAsync(Guid requestIdentifier, CancellationToken cancellationToken)
    {
        var request = await _dbContext.PrivacyRequests
            .AsNoTracking()
            .SingleOrDefaultAsync(candidate => candidate.Id == requestIdentifier, cancellationToken);

        return request is null ? null : PrivacyRequestMapping.ToView(request);
    }
}
