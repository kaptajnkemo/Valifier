using System.Security.Claims;
using Valifier.Domain.Identity;

namespace Valifier.Web.Pages.Shared.Navigation;

public static class TenantWorkspaceNavigation
{
    public static WorkspaceNavigationModel ForDashboard(ClaimsPrincipal user)
    {
        return new WorkspaceNavigationModel(
            null,
            BuildDashboardLinks(user),
            []);
    }

    public static WorkspaceNavigationModel ForUsers()
    {
        return new WorkspaceNavigationModel(
            null,
            [
                new WorkspaceNavigationLink("Tenant dashboard", "/tenant/dashboard"),
                new WorkspaceNavigationLink("Sources of truth", "/tenant/sots"),
                new WorkspaceNavigationLink("Projects", "/tenant/projects")
            ],
            []);
    }

    public static WorkspaceNavigationModel ForUsersCreate()
    {
        return BuildReturnNavigation("Users", "/tenant/users");
    }

    public static WorkspaceNavigationModel ForSourcesOfTruth()
    {
        return new WorkspaceNavigationModel(
            null,
            [
                new WorkspaceNavigationLink("Tenant dashboard", "/tenant/dashboard"),
                new WorkspaceNavigationLink("Users", "/tenant/users"),
                new WorkspaceNavigationLink("Projects", "/tenant/projects")
            ],
            []);
    }

    public static WorkspaceNavigationModel ForSourceOfTruthCreate()
    {
        return BuildReturnNavigation("Sources of truth", "/tenant/sots");
    }

    public static WorkspaceNavigationModel ForSourceOfTruthDetails()
    {
        return BuildReturnNavigation("Sources of truth", "/tenant/sots");
    }

    public static WorkspaceNavigationModel ForProjects(ClaimsPrincipal user)
    {
        return new WorkspaceNavigationModel(
            null,
            BuildProjectLinks(user),
            []);
    }

    public static WorkspaceNavigationModel ForProjectCreate()
    {
        return BuildReturnNavigation("Projects", "/tenant/projects");
    }

    public static WorkspaceNavigationModel ForProjectDetails()
    {
        return BuildReturnNavigation("Projects", "/tenant/projects");
    }

    private static WorkspaceNavigationModel BuildReturnNavigation(string sectionLabel, string sectionHref)
    {
        return new WorkspaceNavigationModel(
            null,
            [],
            [
                new WorkspaceNavigationLink("Tenant dashboard", "/tenant/dashboard"),
                new WorkspaceNavigationLink(sectionLabel, sectionHref)
            ]);
    }

    private static IReadOnlyList<WorkspaceNavigationLink> BuildDashboardLinks(ClaimsPrincipal user)
    {
        if (user.IsInRole(RoleNames.Superuser))
        {
            return
            [
                new WorkspaceNavigationLink("Users", "/tenant/users"),
                new WorkspaceNavigationLink("Sources of truth", "/tenant/sots"),
                new WorkspaceNavigationLink("Projects", "/tenant/projects")
            ];
        }

        return [new WorkspaceNavigationLink("Projects", "/tenant/projects")];
    }

    private static IReadOnlyList<WorkspaceNavigationLink> BuildProjectLinks(ClaimsPrincipal user)
    {
        if (user.IsInRole(RoleNames.Superuser))
        {
            return
            [
                new WorkspaceNavigationLink("Tenant dashboard", "/tenant/dashboard"),
                new WorkspaceNavigationLink("Users", "/tenant/users"),
                new WorkspaceNavigationLink("Sources of truth", "/tenant/sots")
            ];
        }

        return [new WorkspaceNavigationLink("Tenant dashboard", "/tenant/dashboard")];
    }
}
