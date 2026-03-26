using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Valifier.Domain.Identity;
using Valifier.Domain.Knowledge;
using Valifier.Domain.Recruitment;
using Valifier.Domain.Tenancy;
using Valifier.Infrastructure.Identity;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Web.IntegrationTests;

public sealed class Epic3AcceptanceTests : IAsyncLifetime
{
    private static readonly Regex RequestVerificationTokenPattern =
        new("name=\"__RequestVerificationToken\"\\s+type=\"hidden\"\\s+value=\"([^\"]+)\"", RegexOptions.Compiled);

    private readonly Epic1WebApplicationFactory _factory = new();

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return _factory.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task Tenant_dashboard_with_two_tenant_users_shows_the_total_tenant_user_count_value_2()
    {
        var tenantContext = await SeedTenantContextAsync(
            "Skyline",
            "Skyline Owner",
            "skyline.owner.epic3@tenant.local",
            "Tenant0001!");

        await SeedTenantUserAsync(
            tenantContext.TenantId,
            "Skyline Hiring Manager",
            "skyline.hm.epic3@tenant.local",
            "Tenant0001!",
            RoleNames.HiringManager);

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, tenantContext.SuperuserEmail, "Tenant0001!");

        var response = await client.GetAsync("/tenant/dashboard");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("data-testid=\"tenant-user-count-value\">2<", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_dashboard_with_a_tenant_superuser_session_shows_users_sources_of_truth_and_projects_actions()
    {
        var tenantContext = await SeedTenantContextAsync(
            "Northwind",
            "Northwind Owner",
            "northwind.owner.epic3@tenant.local",
            "Tenant0001!");

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, tenantContext.SuperuserEmail, "Tenant0001!");

        var response = await client.GetAsync("/tenant/dashboard");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("href=\"/tenant/users\"", content, StringComparison.Ordinal);
        Assert.Contains("Users", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/sots\"", content, StringComparison.Ordinal);
        Assert.Contains("Sources of truth", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/projects\"", content, StringComparison.Ordinal);
        Assert.Contains("Projects", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Valid_hiring_manager_credentials_redirect_to_the_tenant_dashboard()
    {
        var tenantContext = await SeedTenantContextAsync(
            "Fabrikam",
            "Fabrikam Owner",
            "fabrikam.owner.epic3@tenant.local",
            "Tenant0001!");

        await SeedTenantUserAsync(
            tenantContext.TenantId,
            "Fabrikam Hiring Manager",
            "fabrikam.hm.epic3@tenant.local",
            "Tenant0001!",
            RoleNames.HiringManager);

        using var client = _factory.CreateClient(allowAutoRedirect: false);

        var response = await PostFormAsync(
            client,
            "/sign-in",
            new Dictionary<string, string>
            {
                ["Input.Email"] = "fabrikam.hm.epic3@tenant.local",
                ["Input.Password"] = "Tenant0001!"
            });

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/tenant/dashboard", GetLocationPath(response));
    }

    [Fact]
    public async Task Tenant_dashboard_with_a_hiring_manager_session_shows_projects_action_and_hides_superuser_actions()
    {
        var tenantContext = await SeedTenantContextAsync(
            "Contoso",
            "Contoso Owner",
            "contoso.owner.epic3@tenant.local",
            "Tenant0001!");

        await SeedTenantUserAsync(
            tenantContext.TenantId,
            "Contoso Hiring Manager",
            "contoso.hm.epic3@tenant.local",
            "Tenant0001!",
            RoleNames.HiringManager);

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, "contoso.hm.epic3@tenant.local", "Tenant0001!");

        var response = await client.GetAsync("/tenant/dashboard");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("href=\"/tenant/projects\"", content, StringComparison.Ordinal);
        Assert.Contains("Projects", content, StringComparison.Ordinal);
        Assert.DoesNotContain("href=\"/tenant/users\"", content, StringComparison.Ordinal);
        Assert.DoesNotContain("href=\"/tenant/sots\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_dashboard_with_three_tenant_sources_of_truth_shows_the_total_source_of_truth_count_value_3()
    {
        var tenantContext = await SeedTenantContextAsync(
            "Adventure Works",
            "Adventure Works Owner",
            "adventure.owner.epic3@tenant.local",
            "Tenant0001!");

        await SeedTenantSourceOfTruthAsync(tenantContext.TenantId, "Hiring", "Interview rubric", "v1");
        await SeedTenantSourceOfTruthAsync(tenantContext.TenantId, "Hiring", "Assessment profile", "v2");
        await SeedTenantSourceOfTruthAsync(tenantContext.TenantId, "Operations", "Team glossary", "v3");

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, tenantContext.SuperuserEmail, "Tenant0001!");

        var response = await client.GetAsync("/tenant/dashboard");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("data-testid=\"tenant-source-of-truth-count-value\">3<", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_dashboard_with_four_tenant_recruitment_projects_shows_the_total_project_count_value_4()
    {
        var tenantContext = await SeedTenantContextAsync(
            "Blue Yonder",
            "Blue Yonder Owner",
            "blue.owner.epic3@tenant.local",
            "Tenant0001!");

        await SeedRecruitmentProjectAsync(tenantContext.TenantId, "Platform Engineer", "Engineering");
        await SeedRecruitmentProjectAsync(tenantContext.TenantId, "Data Analyst", "Operations");
        await SeedRecruitmentProjectAsync(tenantContext.TenantId, "Finance Manager", "Finance");
        await SeedRecruitmentProjectAsync(tenantContext.TenantId, "Support Lead", "Customer Support");

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, tenantContext.SuperuserEmail, "Tenant0001!");

        var response = await client.GetAsync("/tenant/dashboard");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("data-testid=\"tenant-project-count-value\">4<", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_superuser_can_open_the_users_route()
    {
        var tenantContext = await SeedTenantContextAsync(
            "Litware",
            "Litware Owner",
            "litware.owner.epic3@tenant.local",
            "Tenant0001!");

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, tenantContext.SuperuserEmail, "Tenant0001!");

        var response = await client.GetAsync("/tenant/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Tenant_superuser_can_open_the_sources_of_truth_route()
    {
        var tenantContext = await SeedTenantContextAsync(
            "Proseware",
            "Proseware Owner",
            "proseware.owner.epic3@tenant.local",
            "Tenant0001!");

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, tenantContext.SuperuserEmail, "Tenant0001!");

        var response = await client.GetAsync("/tenant/sots");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Tenant_superuser_can_open_the_projects_route()
    {
        var tenantContext = await SeedTenantContextAsync(
            "Woodgrove",
            "Woodgrove Owner",
            "woodgrove.owner.epic3@tenant.local",
            "Tenant0001!");

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, tenantContext.SuperuserEmail, "Tenant0001!");

        var response = await client.GetAsync("/tenant/projects");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Hiring_manager_can_open_the_projects_route()
    {
        var tenantContext = await SeedTenantContextAsync(
            "Tailspin",
            "Tailspin Owner",
            "tailspin.owner.epic3@tenant.local",
            "Tenant0001!");

        await SeedTenantUserAsync(
            tenantContext.TenantId,
            "Tailspin Hiring Manager",
            "tailspin.hm.epic3@tenant.local",
            "Tenant0001!",
            RoleNames.HiringManager);

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, "tailspin.hm.epic3@tenant.local", "Tenant0001!");

        var response = await client.GetAsync("/tenant/projects");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<TenantContext> SeedTenantContextAsync(
        string tenantName,
        string superuserDisplayName,
        string superuserEmail,
        string superuserPassword)
    {
        await _factory.SeedTenantAsync(
            tenantName,
            superuserDisplayName,
            superuserEmail,
            superuserPassword,
            hasCompletedFirstSignIn: false);

        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ValifierDbContext>();

        var superuser = await dbContext.Users
            .Where(candidate => candidate.Email == superuserEmail)
            .Select(candidate => new
            {
                candidate.Id,
                candidate.TenantId
            })
            .SingleAsync();

        return new TenantContext(superuser.TenantId!.Value, superuserEmail);
    }

    private async Task SeedTenantUserAsync(
        Guid tenantId,
        string displayName,
        string email,
        string password,
        string roleName)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = new ApplicationUser
        {
            DisplayName = displayName,
            Email = email,
            UserName = email,
            TenantId = tenantId
        };

        var createResult = await userManager.CreateAsync(user, password);
        Assert.True(createResult.Succeeded, string.Join("; ", createResult.Errors.Select(error => error.Description)));

        var roleResult = await userManager.AddToRoleAsync(user, roleName);
        Assert.True(roleResult.Succeeded, string.Join("; ", roleResult.Errors.Select(error => error.Description)));
    }

    private async Task SeedTenantSourceOfTruthAsync(
        Guid tenantId,
        string topic,
        string name,
        string schemaVersion)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ValifierDbContext>();

        dbContext.TenantSourceOfTruths.Add(
            new TenantSourceOfTruth(
                TenantSourceOfTruthId.New(),
                new TenantId(tenantId),
                topic,
                name,
                schemaVersion));

        await dbContext.SaveChangesAsync();
    }

    private async Task SeedRecruitmentProjectAsync(Guid tenantId, string title, string department)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ValifierDbContext>();

        dbContext.RecruitmentProjects.Add(
            new RecruitmentProject(
                RecruitmentProjectId.New(),
                new TenantId(tenantId),
                title,
                department,
                RecruitmentProjectStatus.Draft,
                DateOnly.FromDateTime(DateTime.UtcNow)));

        await dbContext.SaveChangesAsync();
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

    private static async Task SignInAsync(HttpClient client, string email, string password)
    {
        var response = await PostFormAsync(
            client,
            "/sign-in",
            new Dictionary<string, string>
            {
                ["Input.Email"] = email,
                ["Input.Password"] = password
            });

        var content = await response.Content.ReadAsStringAsync();
        Assert.True(response.StatusCode == HttpStatusCode.Redirect, content);
    }

    private static string? GetLocationPath(HttpResponseMessage response)
    {
        var location = response.Headers.Location;
        return location?.IsAbsoluteUri == true
            ? location.PathAndQuery
            : location?.OriginalString;
    }

    private sealed record TenantContext(Guid TenantId, string SuperuserEmail);
}
