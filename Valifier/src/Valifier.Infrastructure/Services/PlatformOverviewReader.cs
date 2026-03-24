using Microsoft.EntityFrameworkCore;
using Valifier.Application.Features.Dashboard;
using Valifier.Domain.Catalog;
using Valifier.Domain.Identity;
using Valifier.Domain.Recruitment;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Infrastructure.Services;

public sealed class PlatformOverviewReader : IPlatformOverviewReader
{
    private readonly ValifierDbContext _dbContext;

    public PlatformOverviewReader(ValifierDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PlatformOverview> GetAsync(CancellationToken cancellationToken)
    {
        var activeProjects = await _dbContext.RecruitmentProjects
            .CountAsync(project => project.Status != RecruitmentProjectStatus.Closed, cancellationToken);

        var modules = SystemModuleCatalog.All
            .Select(module => new PlatformModuleOverview(module.Name, module.Purpose, module.JourneyStage, module.Route))
            .ToArray();

        var roles = RoleCatalog.All
            .Select(role => new PlatformRoleOverview(role.Name, role.Description))
            .ToArray();

        var metrics = new[]
        {
            new PlatformMetric("Active projects", activeProjects.ToString(), "Live recruitment workspaces in the tenant boundary."),
            new PlatformMetric("Structured modules", modules.Length.ToString(), "Discrete system capabilities from role design to decision support."),
            new PlatformMetric("Operational roles", roles.Length.ToString(), "Identity roles seeded for platform, tenant, and hiring workflows.")
        };

        return new PlatformOverview(
            "Valifier",
            "A recruitment workspace that keeps early hiring structured, explainable, and role-specific.",
            metrics,
            modules,
            roles);
    }
}
