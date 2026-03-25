using Microsoft.EntityFrameworkCore;
using Valifier.Application.Features.Tenants.TenantProjects;
using Valifier.Domain.Tenancy;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Infrastructure.Services;

public sealed class TenantProjectDirectoryReader : ITenantProjectDirectoryReader
{
    private readonly ValifierDbContext _dbContext;

    public TenantProjectDirectoryReader(ValifierDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TenantProjectDirectoryView?> GetAsync(Guid userId, CancellationToken cancellationToken)
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

        var projects = await _dbContext.RecruitmentProjects
            .Where(candidate => candidate.TenantId == tenantId)
            .OrderBy(candidate => candidate.Title)
            .Select(candidate => new TenantProjectDirectoryRow(
                candidate.Id.Value,
                candidate.Title,
                candidate.Department,
                candidate.Status.ToString()))
            .ToArrayAsync(cancellationToken);

        return new TenantProjectDirectoryView(projects.Length, projects);
    }
}
