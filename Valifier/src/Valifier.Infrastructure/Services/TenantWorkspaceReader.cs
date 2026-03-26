using Microsoft.EntityFrameworkCore;
using Valifier.Domain.Knowledge;
using Valifier.Application.Features.Tenants.TenantWorkspace;
using Valifier.Domain.Tenancy;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Infrastructure.Services;

public sealed class TenantWorkspaceReader : ITenantWorkspaceReader
{
    private readonly ValifierDbContext _dbContext;

    public TenantWorkspaceReader(ValifierDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TenantWorkspaceView?> GetAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Where(candidate => candidate.Id == userId && candidate.TenantId != null)
            .Select(candidate => new
            {
                candidate.Email,
                candidate.TenantId
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (user?.TenantId is null)
        {
            return null;
        }

        var tenant = await _dbContext.Tenants
            .Where(candidate => candidate.Id == new TenantId(user.TenantId.Value))
            .Select(candidate => new
            {
                candidate.Name,
                TotalTenantUsers = _dbContext.Users.Count(tenantUser => tenantUser.TenantId == user.TenantId.Value),
                TotalTenantSourcesOfTruth = _dbContext.TenantSourceOfTruths.Count(sourceOfTruth => sourceOfTruth.TenantId == candidate.Id),
                TotalTenantProjects = _dbContext.RecruitmentProjects.Count(project => project.TenantId == candidate.Id)
            })
            .SingleOrDefaultAsync(cancellationToken);

        return tenant is null || string.IsNullOrWhiteSpace(user.Email)
            ? null
            : new TenantWorkspaceView(
                tenant.Name,
                user.Email,
                tenant.TotalTenantUsers,
                tenant.TotalTenantSourcesOfTruth,
                tenant.TotalTenantProjects);
    }
}
