namespace Valifier.Web.Pages.Shared.Navigation;

public static class AdminWorkspaceNavigation
{
    public static WorkspaceNavigationModel ForDashboard()
    {
        return new WorkspaceNavigationModel(
            "Admin workspace",
            [],
            []);
    }

    public static WorkspaceNavigationModel ForTenantCreate()
    {
        return new WorkspaceNavigationModel(
            null,
            [],
            [new WorkspaceNavigationLink("Admin dashboard", "/admin/dashboard")]);
    }

    public static WorkspaceNavigationModel ForTenantDetails()
    {
        return new WorkspaceNavigationModel(
            null,
            [],
            [
                new WorkspaceNavigationLink("Admin dashboard", "/admin/dashboard"),
                new WorkspaceNavigationLink("Back to tenants", "/admin/dashboard")
            ]);
    }
}
