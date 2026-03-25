using Microsoft.EntityFrameworkCore;
using Valifier.Application.Features.Tenants.TenantSourceOfTruths;
using Valifier.Domain.Knowledge;
using Valifier.Domain.Tenancy;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Infrastructure.Services;

public sealed class TenantSourceOfTruthDetailReader : ITenantSourceOfTruthDetailReader
{
    private readonly ValifierDbContext _dbContext;

    public TenantSourceOfTruthDetailReader(ValifierDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TenantSourceOfTruthDetailView?> GetAsync(
        Guid actorUserId,
        Guid sourceOfTruthId,
        CancellationToken cancellationToken)
    {
        var tenantIdValue = await _dbContext.Users
            .Where(candidate => candidate.Id == actorUserId && candidate.TenantId != null)
            .Select(candidate => candidate.TenantId)
            .SingleOrDefaultAsync(cancellationToken);

        if (!tenantIdValue.HasValue)
        {
            return null;
        }

        var tenantId = new TenantId(tenantIdValue.Value);
        var sourceOfTruthIdentifier = new TenantSourceOfTruthId(sourceOfTruthId);

        var sourceOfTruth = await _dbContext.TenantSourceOfTruths
            .Where(candidate => candidate.Id == sourceOfTruthIdentifier && candidate.TenantId == tenantId)
            .Select(candidate => new
            {
                candidate.Id.Value,
                candidate.Topic,
                candidate.Name,
                candidate.SchemaVersion
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (sourceOfTruth is null)
        {
            return null;
        }

        var entries = await _dbContext.TenantSourceOfTruthEntries
            .Where(entry => entry.SourceOfTruthId == sourceOfTruthIdentifier)
            .OrderBy(entry => entry.Key)
            .Select(entry => new TenantSourceOfTruthEntryView(
                entry.Key,
                entry.Label,
                entry.ValueType,
                entry.Value))
            .ToArrayAsync(cancellationToken);

        return new TenantSourceOfTruthDetailView(
            sourceOfTruth.Value,
            sourceOfTruth.Topic,
            sourceOfTruth.Name,
            sourceOfTruth.SchemaVersion,
            entries);
    }
}
