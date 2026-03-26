using System.Net;
using System.Text.RegularExpressions;

namespace Valifier.Web.IntegrationTests;

public sealed class Epic4AdminNavigationAcceptanceTests : IAsyncLifetime
{
    private static readonly Regex RequestVerificationTokenPattern =
        new("name=\"__RequestVerificationToken\"\\s+type=\"hidden\"\\s+value=\"([^\"]+)\"", RegexOptions.Compiled);

    private readonly Epic1WebApplicationFactory _factory = new();

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _factory.DisposeAsync().AsTask();

    [Fact]
    public async Task Admin_dashboard_shows_a_visible_current_location_label_for_the_admin_workspace()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsAdminAsync(client);

        var response = await client.GetAsync("/admin/dashboard");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Admin workspace", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Admin_dashboard_visible_navigation_action_opens_the_tenant_creation_route()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsAdminAsync(client);

        var dashboardContent = await client.GetStringAsync("/admin/dashboard");
        var createTenantPath = ExtractAnchorHref(dashboardContent, "Create tenant");

        var response = await client.GetAsync(createTenantPath);

        Assert.Equal("/admin/tenants/new", createTenantPath);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Tenant_creation_page_visible_admin_dashboard_return_action_opens_the_admin_dashboard()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsAdminAsync(client);

        var createTenantContent = await client.GetStringAsync("/admin/tenants/new");
        var adminDashboardPath = ExtractAnchorHref(createTenantContent, "Admin dashboard");

        var response = await client.GetAsync(adminDashboardPath);

        Assert.Equal("/admin/dashboard", adminDashboardPath);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Tenant_detail_page_visible_admin_dashboard_return_action_opens_the_admin_dashboard()
    {
        await _factory.SeedTenantAsync("Northwind", "Northwind Owner", "northwind.owner.epic4@tenant.local", "Tenant0001!", false);

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsAdminAsync(client);

        var tenantDetailPath = ExtractTenantDetailPath(await client.GetStringAsync("/admin/dashboard"));
        var tenantDetailContent = await client.GetStringAsync(tenantDetailPath);
        var adminDashboardPath = ExtractAnchorHref(tenantDetailContent, "Admin dashboard");

        var response = await client.GetAsync(adminDashboardPath);

        Assert.Equal("/admin/dashboard", adminDashboardPath);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Tenant_detail_page_visible_back_to_tenants_action_opens_the_admin_dashboard()
    {
        await _factory.SeedTenantAsync("Fabrikam", "Fabrikam Owner", "fabrikam.owner.epic4@tenant.local", "Tenant0001!", false);

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsAdminAsync(client);

        var tenantDetailPath = ExtractTenantDetailPath(await client.GetStringAsync("/admin/dashboard"));
        var tenantDetailContent = await client.GetStringAsync(tenantDetailPath);
        var backToTenantsPath = ExtractAnchorHref(tenantDetailContent, "Back to tenants");

        var response = await client.GetAsync(backToTenantsPath);

        Assert.Equal("/admin/dashboard", backToTenantsPath);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Tenant_creation_route_without_an_authenticated_global_admin_session_redirects_to_sign_in()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);

        var response = await client.GetAsync("/admin/tenants/new");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/sign-in", GetLocationPath(response), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_detail_route_without_an_authenticated_global_admin_session_redirects_to_sign_in()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);

        var response = await client.GetAsync($"/admin/tenants/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/sign-in", GetLocationPath(response), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Global_admin_can_navigate_from_dashboard_to_create_tenant_and_back_using_visible_in_app_navigation_actions_only()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsAdminAsync(client);

        var dashboardContent = await client.GetStringAsync("/admin/dashboard");
        var createTenantPath = ExtractAnchorHref(dashboardContent, "Create tenant");
        var createTenantContent = await client.GetStringAsync(createTenantPath);
        var adminDashboardPath = ExtractAnchorHref(createTenantContent, "Admin dashboard");

        var dashboardResponse = await client.GetAsync(adminDashboardPath);

        Assert.Equal("/admin/tenants/new", createTenantPath);
        Assert.Equal("/admin/dashboard", adminDashboardPath);
        Assert.Equal(HttpStatusCode.OK, dashboardResponse.StatusCode);
    }

    [Fact]
    public async Task Global_admin_can_navigate_from_dashboard_to_tenant_detail_and_back_using_visible_in_app_navigation_actions_only()
    {
        await _factory.SeedTenantAsync("Contoso", "Contoso Owner", "contoso.owner.epic4@tenant.local", "Tenant0001!", false);

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsAdminAsync(client);

        var dashboardContent = await client.GetStringAsync("/admin/dashboard");
        var tenantDetailPath = ExtractTenantDetailPath(dashboardContent);
        var tenantDetailContent = await client.GetStringAsync(tenantDetailPath);
        var adminDashboardPath = ExtractAnchorHref(tenantDetailContent, "Back to tenants");

        var dashboardResponse = await client.GetAsync(adminDashboardPath);

        Assert.StartsWith("/admin/tenants/", tenantDetailPath, StringComparison.Ordinal);
        Assert.Equal("/admin/dashboard", adminDashboardPath);
        Assert.Equal(HttpStatusCode.OK, dashboardResponse.StatusCode);
    }

    private async Task SignInAsAdminAsync(HttpClient client)
    {
        var response = await PostFormAsync(
            client,
            "/sign-in",
            new Dictionary<string, string>
            {
                ["Input.Email"] = _factory.AdminEmail,
                ["Input.Password"] = _factory.AdminPassword
            });

        var content = await response.Content.ReadAsStringAsync();
        Assert.True(response.StatusCode == HttpStatusCode.Redirect, content);
    }

    private static string ExtractTenantDetailPath(string content)
    {
        var match = Regex.Match(
            content,
            "href=\"(/admin/tenants/[0-9a-fA-F\\-]{36})\"",
            RegexOptions.CultureInvariant);

        Assert.True(match.Success, "Expected the dashboard to contain a tenant detail link.");
        return match.Groups[1].Value;
    }

    private static string ExtractAnchorHref(string content, string linkText)
    {
        var match = Regex.Match(
            content,
            $"<a[^>]*href=\"([^\"]+)\"[^>]*>\\s*{Regex.Escape(linkText)}\\s*</a>",
            RegexOptions.CultureInvariant);

        Assert.True(match.Success, $"Expected a link with text '{linkText}'.");
        return WebUtility.HtmlDecode(match.Groups[1].Value);
    }

    private static async Task<HttpResponseMessage> PostFormAsync(
        HttpClient client,
        string path,
        Dictionary<string, string> values)
    {
        var tokenResponse = await client.GetAsync(path);
        var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
        var tokenMatch = RequestVerificationTokenPattern.Match(tokenContent);

        Assert.True(tokenMatch.Success, $"Expected an antiforgery token in the response for '{path}'.");
        values["__RequestVerificationToken"] = tokenMatch.Groups[1].Value;

        return await client.PostAsync(path, new FormUrlEncodedContent(values));
    }

    private static string? GetLocationPath(HttpResponseMessage response)
    {
        var location = response.Headers.Location;
        return location?.IsAbsoluteUri == true
            ? location.PathAndQuery
            : location?.OriginalString;
    }
}

public sealed class Epic4TenantNavigationAcceptanceTests : Epic3WorkflowAcceptanceTestBase
{
    [Fact]
    public async Task Tenant_dashboard_with_a_tenant_superuser_session_shows_visible_navigation_actions_to_users_sources_of_truth_and_projects()
    {
        var tenant = await SeedTenantContextAsync("Northwind", "Northwind Owner", "northwind.owner.nav@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var content = await client.GetStringAsync("/tenant/dashboard");

        Assert.Contains("href=\"/tenant/users\"", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/sots\"", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/projects\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_users_page_with_a_tenant_superuser_session_shows_visible_navigation_actions_to_dashboard_sources_of_truth_and_projects()
    {
        var tenant = await SeedTenantContextAsync("Graphic Design", "Graphic Design Owner", "graphic.owner.nav@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var content = await client.GetStringAsync("/tenant/users");

        Assert.Contains("href=\"/tenant/dashboard\"", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/sots\"", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/projects\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_sources_of_truth_page_with_a_tenant_superuser_session_shows_visible_navigation_actions_to_dashboard_users_and_projects()
    {
        var tenant = await SeedTenantContextAsync("Proseware", "Proseware Owner", "proseware.owner.nav@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var content = await client.GetStringAsync("/tenant/sots");

        Assert.Contains("href=\"/tenant/dashboard\"", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/users\"", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/projects\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_projects_page_with_a_tenant_superuser_session_shows_visible_navigation_actions_to_dashboard_users_and_sources_of_truth()
    {
        var tenant = await SeedTenantContextAsync("Projects", "Projects Owner", "projects.owner.nav@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "Projects HM", "projects.hm.nav@tenant.local", "Tenant0001!");
        await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Projects guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var content = await client.GetStringAsync("/tenant/projects");

        Assert.Contains("href=\"/tenant/dashboard\"", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/users\"", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/sots\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_user_creation_page_with_a_tenant_superuser_session_shows_visible_tenant_dashboard_and_users_return_actions()
    {
        var tenant = await SeedTenantContextAsync("Users", "Users Owner", "users.owner.nav@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var content = await client.GetStringAsync("/tenant/users/new");

        Assert.Contains("Tenant dashboard", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/dashboard\"", content, StringComparison.Ordinal);
        Assert.Contains(">Users<", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/users\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_source_of_truth_creation_page_with_a_tenant_superuser_session_shows_visible_tenant_dashboard_and_sources_of_truth_return_actions()
    {
        var tenant = await SeedTenantContextAsync("Sots", "Sots Owner", "sots.owner.nav@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var content = await client.GetStringAsync("/tenant/sots/new");

        Assert.Contains("Tenant dashboard", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/dashboard\"", content, StringComparison.Ordinal);
        Assert.Contains("Sources of truth", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/sots\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_source_of_truth_detail_page_with_a_tenant_superuser_session_shows_visible_tenant_dashboard_and_sources_of_truth_return_actions()
    {
        var tenant = await SeedTenantContextAsync("Detail Sots", "Detail Sots Owner", "detailsots.owner.nav@tenant.local", "Tenant0001!");
        await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Detail guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var detailPath = ExtractFirstPath(await client.GetStringAsync("/tenant/sots"), "/tenant/sots/");
        var content = await client.GetStringAsync(detailPath);

        Assert.Contains("Tenant dashboard", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/dashboard\"", content, StringComparison.Ordinal);
        Assert.Contains("Sources of truth", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/sots\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_project_creation_page_with_a_tenant_superuser_session_shows_visible_tenant_dashboard_and_projects_return_actions()
    {
        var tenant = await SeedTenantContextAsync("New Projects", "New Projects Owner", "newprojects.owner.nav@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "New Projects HM", "newprojects.hm.nav@tenant.local", "Tenant0001!");
        await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "New Projects guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var content = await client.GetStringAsync("/tenant/projects/new");

        Assert.Contains("Tenant dashboard", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/dashboard\"", content, StringComparison.Ordinal);
        Assert.Contains(">Projects<", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/projects\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_project_detail_page_with_a_tenant_superuser_session_shows_visible_tenant_dashboard_and_projects_return_actions()
    {
        var tenant = await SeedTenantContextAsync("Project Detail", "Project Detail Owner", "projectdetail.owner.nav@tenant.local", "Tenant0001!");
        var owner = await SeedHiringManagerDirectAsync(tenant.TenantId, "Project Detail HM", "projectdetail.hm.nav@tenant.local", "Tenant0001!");
        var source = await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Project Detail guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var detailPath = await CreateProjectThroughUiAsync(client, "Project Detail", "Engineering", source.Id.Value, owner.Id);
        var content = await client.GetStringAsync(detailPath);

        Assert.Contains("Tenant dashboard", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/dashboard\"", content, StringComparison.Ordinal);
        Assert.Contains(">Projects<", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/projects\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_user_creation_route_without_an_authenticated_tenant_superuser_session_redirects_to_sign_in()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/tenant/users/new");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/sign-in", GetLocationPath(response), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_source_of_truth_creation_route_without_an_authenticated_tenant_superuser_session_redirects_to_sign_in()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/tenant/sots/new");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/sign-in", GetLocationPath(response), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_project_creation_route_without_an_authenticated_tenant_workspace_session_redirects_to_sign_in()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/tenant/projects/new");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/sign-in", GetLocationPath(response), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_dashboard_with_a_hiring_manager_session_shows_a_visible_navigation_action_to_projects()
    {
        var tenant = await SeedTenantContextAsync("Hiring Dashboard", "Hiring Dashboard Owner", "hiringdash.owner.nav@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "Hiring Dashboard HM", "hiringdash.hm.nav@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, "hiringdash.hm.nav@tenant.local", "Tenant0001!");

        var content = await client.GetStringAsync("/tenant/dashboard");

        Assert.Contains("href=\"/tenant/projects\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_projects_page_with_a_hiring_manager_session_shows_a_visible_navigation_action_to_the_tenant_dashboard()
    {
        var tenant = await SeedTenantContextAsync("Hiring Projects", "Hiring Projects Owner", "hiringprojects.owner.nav@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "Hiring Projects HM", "hiringprojects.hm.nav@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, "hiringprojects.hm.nav@tenant.local", "Tenant0001!");

        var content = await client.GetStringAsync("/tenant/projects");

        Assert.Contains("href=\"/tenant/dashboard\"", content, StringComparison.Ordinal);
        Assert.Contains("Tenant dashboard", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_project_creation_page_with_a_hiring_manager_session_shows_visible_tenant_dashboard_and_projects_return_actions()
    {
        var tenant = await SeedTenantContextAsync("Hiring New Project", "Hiring New Project Owner", "hiringnewproject.owner.nav@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "Hiring New Project HM", "hiringnewproject.hm.nav@tenant.local", "Tenant0001!");
        await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Hiring New Project guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, "hiringnewproject.hm.nav@tenant.local", "Tenant0001!");

        var content = await client.GetStringAsync("/tenant/projects/new");

        Assert.Contains("Tenant dashboard", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/dashboard\"", content, StringComparison.Ordinal);
        Assert.Contains(">Projects<", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/projects\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_project_detail_page_with_a_hiring_manager_session_shows_visible_tenant_dashboard_and_projects_return_actions()
    {
        var tenant = await SeedTenantContextAsync("Hiring Detail Project", "Hiring Detail Project Owner", "hiringdetailproject.owner.nav@tenant.local", "Tenant0001!");
        var owner = await SeedHiringManagerDirectAsync(tenant.TenantId, "Hiring Detail Project HM", "hiringdetailproject.hm.nav@tenant.local", "Tenant0001!");
        var source = await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Hiring Detail Project guide", "v1");

        using var superuserClient = CreateClient();
        await SignInAsync(superuserClient, tenant.SuperuserEmail, tenant.SuperuserPassword);
        var detailPath = await CreateProjectThroughUiAsync(superuserClient, "Hiring Detail Project", "Engineering", source.Id.Value, owner.Id);

        using var hiringManagerClient = CreateClient();
        await SignInAsync(hiringManagerClient, "hiringdetailproject.hm.nav@tenant.local", "Tenant0001!");

        var content = await hiringManagerClient.GetStringAsync(detailPath);

        Assert.Contains("Tenant dashboard", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/dashboard\"", content, StringComparison.Ordinal);
        Assert.Contains(">Projects<", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/projects\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_projects_page_with_a_hiring_manager_session_does_not_show_a_visible_navigation_action_to_users()
    {
        var tenant = await SeedTenantContextAsync("No Users Projects", "No Users Projects Owner", "nouserprojects.owner.nav@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "No Users Projects HM", "nouserprojects.hm.nav@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, "nouserprojects.hm.nav@tenant.local", "Tenant0001!");

        var content = await client.GetStringAsync("/tenant/projects");

        Assert.DoesNotContain("href=\"/tenant/users\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_projects_page_with_a_hiring_manager_session_does_not_show_a_visible_navigation_action_to_sources_of_truth()
    {
        var tenant = await SeedTenantContextAsync("No Sots Projects", "No Sots Projects Owner", "nosotsprojects.owner.nav@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "No Sots Projects HM", "nosotsprojects.hm.nav@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, "nosotsprojects.hm.nav@tenant.local", "Tenant0001!");

        var content = await client.GetStringAsync("/tenant/projects");

        Assert.DoesNotContain("href=\"/tenant/sots\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_project_creation_page_with_a_hiring_manager_session_does_not_show_a_visible_navigation_action_to_users()
    {
        var tenant = await SeedTenantContextAsync("No Users Create", "No Users Create Owner", "nousercreate.owner.nav@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "No Users Create HM", "nousercreate.hm.nav@tenant.local", "Tenant0001!");
        await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "No Users Create guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, "nousercreate.hm.nav@tenant.local", "Tenant0001!");

        var content = await client.GetStringAsync("/tenant/projects/new");

        Assert.DoesNotContain("href=\"/tenant/users\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_project_creation_page_with_a_hiring_manager_session_does_not_show_a_visible_navigation_action_to_sources_of_truth()
    {
        var tenant = await SeedTenantContextAsync("No Sots Create", "No Sots Create Owner", "nosotscreate.owner.nav@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "No Sots Create HM", "nosotscreate.hm.nav@tenant.local", "Tenant0001!");
        await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "No Sots Create guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, "nosotscreate.hm.nav@tenant.local", "Tenant0001!");

        var content = await client.GetStringAsync("/tenant/projects/new");

        Assert.DoesNotContain("href=\"/tenant/sots\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_project_detail_page_with_a_hiring_manager_session_does_not_show_a_visible_navigation_action_to_users()
    {
        var tenant = await SeedTenantContextAsync("No Users Detail", "No Users Detail Owner", "nouserdetail.owner.nav@tenant.local", "Tenant0001!");
        var owner = await SeedHiringManagerDirectAsync(tenant.TenantId, "No Users Detail HM", "nouserdetail.hm.nav@tenant.local", "Tenant0001!");
        var source = await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "No Users Detail guide", "v1");

        using var superuserClient = CreateClient();
        await SignInAsync(superuserClient, tenant.SuperuserEmail, tenant.SuperuserPassword);
        var detailPath = await CreateProjectThroughUiAsync(superuserClient, "No Users Detail", "Engineering", source.Id.Value, owner.Id);

        using var client = CreateClient();
        await SignInAsync(client, "nouserdetail.hm.nav@tenant.local", "Tenant0001!");

        var content = await client.GetStringAsync(detailPath);

        Assert.DoesNotContain("href=\"/tenant/users\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_project_detail_page_with_a_hiring_manager_session_does_not_show_a_visible_navigation_action_to_sources_of_truth()
    {
        var tenant = await SeedTenantContextAsync("No Sots Detail", "No Sots Detail Owner", "nosotsdetail.owner.nav@tenant.local", "Tenant0001!");
        var owner = await SeedHiringManagerDirectAsync(tenant.TenantId, "No Sots Detail HM", "nosotsdetail.hm.nav@tenant.local", "Tenant0001!");
        var source = await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "No Sots Detail guide", "v1");

        using var superuserClient = CreateClient();
        await SignInAsync(superuserClient, tenant.SuperuserEmail, tenant.SuperuserPassword);
        var detailPath = await CreateProjectThroughUiAsync(superuserClient, "No Sots Detail", "Engineering", source.Id.Value, owner.Id);

        using var client = CreateClient();
        await SignInAsync(client, "nosotsdetail.hm.nav@tenant.local", "Tenant0001!");

        var content = await client.GetStringAsync(detailPath);

        Assert.DoesNotContain("href=\"/tenant/sots\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_superuser_can_navigate_from_dashboard_to_users_to_user_creation_and_back_to_dashboard_using_visible_in_app_navigation_actions_only()
    {
        var tenant = await SeedTenantContextAsync("Workflow Users", "Workflow Users Owner", "workflowusers.owner.nav@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var dashboardContent = await client.GetStringAsync("/tenant/dashboard");
        var usersPath = ExtractAnchorHref(dashboardContent, "Users");
        var usersContent = await client.GetStringAsync(usersPath);
        var usersNewPath = ExtractAnchorHref(usersContent, "Create Hiring Manager");
        var usersNewContent = await client.GetStringAsync(usersNewPath);
        var returnToUsersPath = ExtractAnchorHref(usersNewContent, "Users");
        var returnToUsersContent = await client.GetStringAsync(returnToUsersPath);
        var returnToDashboardPath = ExtractAnchorHref(returnToUsersContent, "Tenant dashboard");

        var dashboardResponse = await client.GetAsync(returnToDashboardPath);

        Assert.Equal("/tenant/users", usersPath);
        Assert.Equal("/tenant/users/new", usersNewPath);
        Assert.Equal("/tenant/users", returnToUsersPath);
        Assert.Equal("/tenant/dashboard", returnToDashboardPath);
        Assert.Equal(HttpStatusCode.OK, dashboardResponse.StatusCode);
    }

    [Fact]
    public async Task Tenant_superuser_can_navigate_from_dashboard_to_sources_of_truth_to_source_creation_and_back_to_dashboard_using_visible_in_app_navigation_actions_only()
    {
        var tenant = await SeedTenantContextAsync("Workflow Sots", "Workflow Sots Owner", "workflowsots.owner.nav@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var dashboardContent = await client.GetStringAsync("/tenant/dashboard");
        var sotsPath = ExtractAnchorHref(dashboardContent, "Sources of truth");
        var sotsContent = await client.GetStringAsync(sotsPath);
        var sotsNewPath = ExtractAnchorHref(sotsContent, "Create source of truth");
        var sotsNewContent = await client.GetStringAsync(sotsNewPath);
        var returnToSotsPath = ExtractAnchorHref(sotsNewContent, "Sources of truth");
        var returnToSotsContent = await client.GetStringAsync(returnToSotsPath);
        var returnToDashboardPath = ExtractAnchorHref(returnToSotsContent, "Tenant dashboard");

        var dashboardResponse = await client.GetAsync(returnToDashboardPath);

        Assert.Equal("/tenant/sots", sotsPath);
        Assert.Equal("/tenant/sots/new", sotsNewPath);
        Assert.Equal("/tenant/sots", returnToSotsPath);
        Assert.Equal("/tenant/dashboard", returnToDashboardPath);
        Assert.Equal(HttpStatusCode.OK, dashboardResponse.StatusCode);
    }

    [Fact]
    public async Task Tenant_superuser_can_navigate_from_dashboard_to_projects_to_project_creation_and_back_to_dashboard_using_visible_in_app_navigation_actions_only()
    {
        var tenant = await SeedTenantContextAsync("Workflow Projects", "Workflow Projects Owner", "workflowprojects.owner.nav@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "Workflow Projects HM", "workflowprojects.hm.nav@tenant.local", "Tenant0001!");
        await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Workflow Projects guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var dashboardContent = await client.GetStringAsync("/tenant/dashboard");
        var projectsPath = ExtractAnchorHref(dashboardContent, "Projects");
        var projectsContent = await client.GetStringAsync(projectsPath);
        var projectsNewPath = ExtractAnchorHref(projectsContent, "Create project");
        var projectsNewContent = await client.GetStringAsync(projectsNewPath);
        var returnToProjectsPath = ExtractAnchorHref(projectsNewContent, "Projects");
        var returnToProjectsContent = await client.GetStringAsync(returnToProjectsPath);
        var returnToDashboardPath = ExtractAnchorHref(returnToProjectsContent, "Tenant dashboard");

        var dashboardResponse = await client.GetAsync(returnToDashboardPath);

        Assert.Equal("/tenant/projects", projectsPath);
        Assert.Equal("/tenant/projects/new", projectsNewPath);
        Assert.Equal("/tenant/projects", returnToProjectsPath);
        Assert.Equal("/tenant/dashboard", returnToDashboardPath);
        Assert.Equal(HttpStatusCode.OK, dashboardResponse.StatusCode);
    }

    [Fact]
    public async Task Tenant_superuser_can_navigate_from_projects_to_project_detail_and_back_to_dashboard_using_visible_in_app_navigation_actions_only()
    {
        var tenant = await SeedTenantContextAsync("Workflow Project Detail", "Workflow Project Detail Owner", "workflowprojectdetail.owner.nav@tenant.local", "Tenant0001!");
        var owner = await SeedHiringManagerDirectAsync(tenant.TenantId, "Workflow Project Detail HM", "workflowprojectdetail.hm.nav@tenant.local", "Tenant0001!");
        var source = await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Workflow Project Detail guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);
        await CreateProjectThroughUiAsync(client, "Workflow Project Detail", "Engineering", source.Id.Value, owner.Id);

        var projectsContent = await client.GetStringAsync("/tenant/projects");
        var detailPath = ExtractAnchorHref(projectsContent, "View project");
        var detailContent = await client.GetStringAsync(detailPath);
        var returnToProjectsPath = ExtractAnchorHref(detailContent, "Projects");
        var returnToProjectsContent = await client.GetStringAsync(returnToProjectsPath);
        var returnToDashboardPath = ExtractAnchorHref(returnToProjectsContent, "Tenant dashboard");

        var dashboardResponse = await client.GetAsync(returnToDashboardPath);

        Assert.StartsWith("/tenant/projects/", detailPath, StringComparison.Ordinal);
        Assert.Equal("/tenant/projects", returnToProjectsPath);
        Assert.Equal("/tenant/dashboard", returnToDashboardPath);
        Assert.Equal(HttpStatusCode.OK, dashboardResponse.StatusCode);
    }

    [Fact]
    public async Task Hiring_manager_can_navigate_from_dashboard_to_projects_to_project_detail_and_back_to_dashboard_using_visible_in_app_navigation_actions_only()
    {
        var tenant = await SeedTenantContextAsync("Workflow Hiring Manager", "Workflow Hiring Manager Owner", "workflowhm.owner.nav@tenant.local", "Tenant0001!");
        var owner = await SeedHiringManagerDirectAsync(tenant.TenantId, "Workflow Hiring Manager HM", "workflowhm.hm.nav@tenant.local", "Tenant0001!");
        var source = await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Workflow Hiring Manager guide", "v1");

        using var superuserClient = CreateClient();
        await SignInAsync(superuserClient, tenant.SuperuserEmail, tenant.SuperuserPassword);
        await CreateProjectThroughUiAsync(superuserClient, "Workflow Hiring Manager", "Engineering", source.Id.Value, owner.Id);

        using var client = CreateClient();
        await SignInAsync(client, "workflowhm.hm.nav@tenant.local", "Tenant0001!");

        var dashboardContent = await client.GetStringAsync("/tenant/dashboard");
        var projectsPath = ExtractAnchorHref(dashboardContent, "Projects");
        var projectsContent = await client.GetStringAsync(projectsPath);
        var detailPath = ExtractAnchorHref(projectsContent, "View project");
        var detailContent = await client.GetStringAsync(detailPath);
        var returnToProjectsPath = ExtractAnchorHref(detailContent, "Projects");
        var returnToProjectsContent = await client.GetStringAsync(returnToProjectsPath);
        var returnToDashboardPath = ExtractAnchorHref(returnToProjectsContent, "Tenant dashboard");

        var dashboardResponse = await client.GetAsync(returnToDashboardPath);

        Assert.Equal("/tenant/projects", projectsPath);
        Assert.StartsWith("/tenant/projects/", detailPath, StringComparison.Ordinal);
        Assert.Equal("/tenant/projects", returnToProjectsPath);
        Assert.Equal("/tenant/dashboard", returnToDashboardPath);
        Assert.Equal(HttpStatusCode.OK, dashboardResponse.StatusCode);
    }

    private static string ExtractAnchorHref(string content, string linkText)
    {
        var match = Regex.Match(
            content,
            $"<a[^>]*href=\"([^\"]+)\"[^>]*>\\s*{Regex.Escape(linkText)}\\s*</a>",
            RegexOptions.CultureInvariant);

        Assert.True(match.Success, $"Expected a link with text '{linkText}'.");
        return WebUtility.HtmlDecode(match.Groups[1].Value);
    }
}
