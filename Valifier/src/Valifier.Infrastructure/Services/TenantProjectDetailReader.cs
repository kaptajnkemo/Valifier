using Microsoft.EntityFrameworkCore;
using Valifier.Application.Features.Tenants.TenantProjects;
using Valifier.Domain.Identity;
using Valifier.Domain.Recruitment;
using Valifier.Domain.Tenancy;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Infrastructure.Services;

public sealed class TenantProjectDetailReader : ITenantProjectDetailReader
{
    private readonly ValifierDbContext _dbContext;

    public TenantProjectDetailReader(ValifierDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TenantProjectDetailView?> GetAsync(Guid actorUserId, Guid projectId, CancellationToken cancellationToken)
    {
        var actor = await _dbContext.Users
            .Where(candidate => candidate.Id == actorUserId && candidate.TenantId != null)
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
        var projectIdentifier = new RecruitmentProjectId(projectId);
        var project = await _dbContext.RecruitmentProjects
            .Where(recruitmentProject => recruitmentProject.Id == projectIdentifier &&
                                         recruitmentProject.TenantId == tenantId)
            .Select(recruitmentProject => new
            {
                recruitmentProject.Id,
                recruitmentProject.Title,
                recruitmentProject.Department,
                recruitmentProject.OwnerUserId,
                recruitmentProject.SourceOfTruthId
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (project is null)
        {
            return null;
        }

        if (actorRoles.Contains(RoleNames.HiringManager, StringComparer.Ordinal) &&
            !actorRoles.Contains(RoleNames.Superuser, StringComparer.Ordinal) &&
            project.OwnerUserId.Value != actor.Id)
        {
            return null;
        }

        var ownerDisplayName = await _dbContext.Users
            .Where(candidate => candidate.Id == project.OwnerUserId.Value)
            .Select(candidate => candidate.DisplayName)
            .SingleOrDefaultAsync(cancellationToken);

        var sourceOfTruthName = await _dbContext.TenantSourceOfTruths
            .Where(candidate => candidate.Id == project.SourceOfTruthId && candidate.TenantId == tenantId)
            .Select(candidate => candidate.Name)
            .SingleOrDefaultAsync(cancellationToken);

        if (sourceOfTruthName is null)
        {
            return null;
        }

        return new TenantProjectDetailView(
            project.Id.Value,
            project.Title,
            project.Department,
            ownerDisplayName ?? string.Empty,
            sourceOfTruthName);
    }
}
