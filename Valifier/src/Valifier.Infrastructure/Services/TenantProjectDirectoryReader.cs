using Microsoft.EntityFrameworkCore;
using Valifier.Application.Features.Tenants.TenantProjects;
using Valifier.Domain.Identity;
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
        var actor = await _dbContext.Users
            .Where(candidate => candidate.Id == userId && candidate.TenantId != null)
            .Select(candidate => new
            {
                candidate.Id,
                candidate.TenantId
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (actor?.TenantId is null)
        {
            return null;
        }

        var actorRoles = await (
            from userRole in _dbContext.UserRoles
            join role in _dbContext.Roles on userRole.RoleId equals role.Id
            where userRole.UserId == actor.Id
            select role.Name ?? string.Empty)
            .ToArrayAsync(cancellationToken);

        var tenantId = new TenantId(actor.TenantId.Value);
        var isHiringManagerOnly = actorRoles.Contains(RoleNames.HiringManager, StringComparer.Ordinal) &&
                                  !actorRoles.Contains(RoleNames.Superuser, StringComparer.Ordinal);
        var actorOwnerUserId = new UserId(actor.Id);

        var projects = await _dbContext.RecruitmentProjects
            .Where(project => project.TenantId == tenantId &&
                              (!isHiringManagerOnly || project.OwnerUserId == actorOwnerUserId))
            .OrderBy(project => project.Title)
            .Select(project => new
            {
                project.Id,
                project.Title,
                project.Department,
                project.OwnerUserId,
                project.Status
            })
            .ToArrayAsync(cancellationToken);

        var ownerIds = projects
            .Select(project => project.OwnerUserId.Value)
            .Where(ownerUserId => ownerUserId != Guid.Empty)
            .Distinct()
            .ToArray();

        var ownerDisplayNames = ownerIds.Length == 0
            ? new Dictionary<Guid, string>()
            : await _dbContext.Users
                .Where(candidate => ownerIds.Contains(candidate.Id))
                .ToDictionaryAsync(candidate => candidate.Id, candidate => candidate.DisplayName, cancellationToken);

        var rows = projects
            .Select(project => new TenantProjectDirectoryRow(
                project.Id.Value,
                project.Title,
                project.Department,
                ownerDisplayNames.GetValueOrDefault(project.OwnerUserId.Value, string.Empty),
                project.Status.ToString()))
            .ToArray();

        return new TenantProjectDirectoryView(rows.Length, rows);
    }
}
