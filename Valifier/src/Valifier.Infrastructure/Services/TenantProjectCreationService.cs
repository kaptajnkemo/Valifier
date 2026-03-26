using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Valifier.Application.Features.Tenants.TenantProjects;
using Valifier.Domain.Identity;
using Valifier.Domain.Knowledge;
using Valifier.Domain.Recruitment;
using Valifier.Domain.Tenancy;
using Valifier.Infrastructure.Identity;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Infrastructure.Services;

public sealed class TenantProjectCreationService : ITenantProjectCreationService
{
    private readonly ValifierDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public TenantProjectCreationService(
        ValifierDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Guid> CreateAsync(CreateTenantProjectCommand command, CancellationToken cancellationToken)
    {
        var actor = await _userManager.FindByIdAsync(command.ActorUserId.ToString("D"));

        if (actor?.TenantId is null)
        {
            throw new InvalidOperationException("A tenant user is required to create a project.");
        }

        var isSuperuser = await _userManager.IsInRoleAsync(actor, RoleNames.Superuser);
        var isHiringManager = await _userManager.IsInRoleAsync(actor, RoleNames.HiringManager);

        if (!isSuperuser && !isHiringManager)
        {
            throw new InvalidOperationException("A tenant role is required to create a project.");
        }

        var ownerUserId = command.OwnerUserId ?? actor.Id;

        if (isSuperuser && !command.OwnerUserId.HasValue)
        {
            throw new InvalidOperationException("A tenant superuser must select one Hiring Manager owner.");
        }

        var owner = await _dbContext.Users
            .Where(candidate => candidate.Id == ownerUserId && candidate.TenantId == actor.TenantId.Value)
            .SingleOrDefaultAsync(cancellationToken);

        if (owner is null)
        {
            throw new InvalidOperationException("The selected Hiring Manager owner was not found.");
        }

        if (!await _userManager.IsInRoleAsync(owner, RoleNames.HiringManager))
        {
            throw new InvalidOperationException("Projects must be owned by a Hiring Manager user.");
        }

        var sourceOfTruthExists = await _dbContext.TenantSourceOfTruths
            .AnyAsync(
                candidate => candidate.Id == new TenantSourceOfTruthId(command.SourceOfTruthId) &&
                             candidate.TenantId == new TenantId(actor.TenantId.Value),
                cancellationToken);

        if (!sourceOfTruthExists)
        {
            throw new InvalidOperationException("The selected source of truth was not found.");
        }

        var project = new RecruitmentProject(
            RecruitmentProjectId.New(),
            new TenantId(actor.TenantId.Value),
            new UserId(owner.Id),
            new TenantSourceOfTruthId(command.SourceOfTruthId),
            command.JobTitle,
            command.Department,
            RecruitmentProjectStatus.Draft,
            DateOnly.FromDateTime(DateTime.UtcNow));

        _dbContext.RecruitmentProjects.Add(project);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return project.Id.Value;
    }
}
