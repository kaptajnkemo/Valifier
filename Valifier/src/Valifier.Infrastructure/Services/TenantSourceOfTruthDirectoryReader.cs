using Microsoft.EntityFrameworkCore;
using Valifier.Application.Features.Tenants.TenantSourceOfTruths;
using Valifier.Domain.Tenancy;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Infrastructure.Services;

public sealed class TenantSourceOfTruthDirectoryReader : ITenantSourceOfTruthDirectoryReader
{
    private readonly ValifierDbContext _dbContext;

    public TenantSourceOfTruthDirectoryReader(ValifierDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TenantSourceOfTruthDirectoryView?> GetAsync(Guid userId, CancellationToken cancellationToken)
    {
        var currentUserTenantId = await _dbContext.Users
            .Where(candidate => candidate.Id == userId && candidate.TenantId != null)
            .Select(candidate => candidate.TenantId)
            .SingleOrDefaultAsync(cancellationToken);

        if (!currentUserTenantId.HasValue)
        {
            return null;
        }

        var tenantId = new TenantId(currentUserTenantId.Value);

        var sourcesOfTruth = await _dbContext.TenantSourceOfTruths
            .Where(candidate => candidate.TenantId == tenantId)
            .OrderBy(candidate => candidate.Topic)
            .ThenBy(candidate => candidate.Name)
            .Select(candidate => new TenantSourceOfTruthDirectoryRow(
                candidate.Id.Value,
                candidate.Topic,
                candidate.Name,
                candidate.SchemaVersion))
            .ToArrayAsync(cancellationToken);

        return new TenantSourceOfTruthDirectoryView(sourcesOfTruth.Length, sourcesOfTruth);
    }
}
