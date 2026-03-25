using Microsoft.EntityFrameworkCore;
using Valifier.Application.Features.Compliance;
using Valifier.Domain.Compliance;
using Valifier.Domain.Tenancy;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Infrastructure.Services;

public sealed class ComplianceMetadataReader : IComplianceMetadataReader
{
    private readonly ValifierDbContext _dbContext;

    public ComplianceMetadataReader(ValifierDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ComplianceMetadataView?> GetAsync(string recordType, Guid recordId, CancellationToken cancellationToken)
    {
        var governedRecord = await LoadGovernedRecordAsync(recordType, recordId, cancellationToken);

        return governedRecord is null
            ? null
            : new ComplianceMetadataView(
                governedRecord.GetRecordIdentifier(),
                governedRecord.GetDataCategoryCode(),
                governedRecord.GetRetentionRuleCode(),
                governedRecord.GetCollectionTimestamp(),
                governedRecord.GetSubjectScope());
    }

    private async Task<IGovernedRecord?> LoadGovernedRecordAsync(
        string recordType,
        Guid recordId,
        CancellationToken cancellationToken)
    {
        return recordType.Trim().ToLowerInvariant() switch
        {
            "tenant" => await _dbContext.Tenants
                .AsNoTracking()
                .SingleOrDefaultAsync(candidate => candidate.Id == new TenantId(recordId), cancellationToken),
            "privacy-request" => await _dbContext.PrivacyRequests
                .AsNoTracking()
                .SingleOrDefaultAsync(candidate => candidate.Id == recordId, cancellationToken),
            _ => null
        };
    }
}
