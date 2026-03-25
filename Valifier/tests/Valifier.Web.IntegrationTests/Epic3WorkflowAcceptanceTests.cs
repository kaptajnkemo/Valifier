using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Valifier.Domain.Identity;
using Valifier.Domain.Knowledge;
using Valifier.Domain.Tenancy;
using Valifier.Infrastructure.Identity;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Web.IntegrationTests;

public abstract class Epic3WorkflowAcceptanceTestBase : IAsyncLifetime
{
    private static readonly Regex RequestVerificationTokenPattern =
        new("name=\"__RequestVerificationToken\"\\s+type=\"hidden\"\\s+value=\"([^\"]+)\"", RegexOptions.Compiled);

    protected Epic1WebApplicationFactory Factory { get; } = new();

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Factory.DisposeAsync().AsTask();

    protected async Task<TenantContext> SeedTenantContextAsync(
        string tenantName,
        string superuserDisplayName,
        string superuserEmail,
        string superuserPassword)
    {
        await Factory.SeedTenantAsync(
            tenantName,
            superuserDisplayName,
            superuserEmail,
            superuserPassword,
            hasCompletedFirstSignIn: false);

        await using var scope = Factory.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var superuser = await userManager.FindByEmailAsync(superuserEmail);

        Assert.NotNull(superuser);
        Assert.True(superuser!.TenantId.HasValue);

        return new TenantContext(superuser.TenantId.Value, superuser.Id, superuserEmail, superuserPassword);
    }

    protected async Task<ApplicationUser> SeedHiringManagerDirectAsync(
        Guid tenantId,
        string displayName,
        string email,
        string password)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
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

        var roleResult = await userManager.AddToRoleAsync(user, RoleNames.HiringManager);
        Assert.True(roleResult.Succeeded, string.Join("; ", roleResult.Errors.Select(error => error.Description)));

        return user;
    }

    protected async Task<TenantSourceOfTruth> SeedSourceOfTruthDirectAsync(
        Guid tenantId,
        string topic,
        string name,
        string schemaVersion)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ValifierDbContext>();

        var sourceOfTruth = new TenantSourceOfTruth(
            TenantSourceOfTruthId.New(),
            new TenantId(tenantId),
            topic,
            name,
            schemaVersion);

        dbContext.TenantSourceOfTruths.Add(sourceOfTruth);
        await dbContext.SaveChangesAsync();

        return sourceOfTruth;
    }

    protected HttpClient CreateClient() => Factory.CreateClient(allowAutoRedirect: false);

    protected static async Task SignInAsync(HttpClient client, string email, string password)
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

    protected static async Task<HttpResponseMessage> PostFormAsync(
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

    protected static async Task<HttpResponseMessage> PostFormAsync(
        HttpClient client,
        string path,
        string tokenPagePath,
        Dictionary<string, string> values)
    {
        var tokenResponse = await client.GetAsync(tokenPagePath);
        var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
        var tokenMatch = RequestVerificationTokenPattern.Match(tokenContent);

        Assert.True(tokenMatch.Success, $"Expected an antiforgery token in the response for '{tokenPagePath}'.");
        values["__RequestVerificationToken"] = tokenMatch.Groups[1].Value;

        return await client.PostAsync(path, new FormUrlEncodedContent(values));
    }

    protected static string? GetLocationPath(HttpResponseMessage response)
    {
        var location = response.Headers.Location;
        return location?.IsAbsoluteUri == true
            ? location.PathAndQuery
            : location?.OriginalString;
    }

    protected static string ExtractFirstPath(string content, string prefix)
    {
        var match = Regex.Match(
            content,
            $"href=\"({Regex.Escape(prefix)}[^\"]+)\"",
            RegexOptions.CultureInvariant);

        Assert.True(match.Success, $"Expected a link starting with '{prefix}'.");
        return WebUtility.HtmlDecode(match.Groups[1].Value);
    }

    protected static string ExtractHiddenValue(string content, string fieldName)
    {
        var match = Regex.Match(
            content,
            $"name=\"{Regex.Escape(fieldName)}\"\\s+type=\"hidden\"\\s+value=\"([^\"]*)\"",
            RegexOptions.CultureInvariant);

        Assert.True(match.Success, $"Expected hidden field '{fieldName}'.");
        return WebUtility.HtmlDecode(match.Groups[1].Value);
    }

    protected static async Task<string> CreateHiringManagerThroughUiAsync(
        HttpClient client,
        string displayName,
        string email,
        string password)
    {
        var response = await PostFormAsync(
            client,
            "/tenant/users/new",
            new Dictionary<string, string>
            {
                ["Input.DisplayName"] = displayName,
                ["Input.Email"] = email,
                ["Input.Password"] = password
            });

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/tenant/users", GetLocationPath(response));
        return email;
    }

    protected static async Task<string> CreateSourceOfTruthThroughUiAsync(
        HttpClient client,
        string topic,
        string name,
        string schemaVersion,
        string entryKey,
        string entryLabel,
        string entryValueType,
        string entryValue)
    {
        var addEntryResponse = await PostFormAsync(
            client,
            "/tenant/sots/new?handler=AddEntry",
            "/tenant/sots/new",
            new Dictionary<string, string>
            {
                ["Input.Topic"] = topic,
                ["Input.Name"] = name,
                ["Input.SchemaVersion"] = schemaVersion,
                ["Input.EntryKey"] = entryKey,
                ["Input.EntryLabel"] = entryLabel,
                ["Input.EntryValueType"] = entryValueType,
                ["Input.EntryValue"] = entryValue,
                ["Input.EntriesJson"] = string.Empty
            });

        var addEntryContent = await addEntryResponse.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, addEntryResponse.StatusCode);

        var entriesJson = ExtractHiddenValue(addEntryContent, "Input.EntriesJson");

        var saveResponse = await PostFormAsync(
            client,
            "/tenant/sots/new?handler=Save",
            "/tenant/sots/new",
            new Dictionary<string, string>
            {
                ["Input.Topic"] = topic,
                ["Input.Name"] = name,
                ["Input.SchemaVersion"] = schemaVersion,
                ["Input.EntriesJson"] = entriesJson
            });

        Assert.Equal(HttpStatusCode.Redirect, saveResponse.StatusCode);
        Assert.Equal("/tenant/sots", GetLocationPath(saveResponse));

        var listContent = await client.GetStringAsync("/tenant/sots");
        return ExtractFirstPath(listContent, "/tenant/sots/");
    }

    protected static async Task<string> CreateProjectThroughUiAsync(
        HttpClient client,
        string jobTitle,
        string department,
        Guid sourceOfTruthId,
        Guid? ownerUserId = null)
    {
        var values = new Dictionary<string, string>
        {
            ["Input.JobTitle"] = jobTitle,
            ["Input.Department"] = department,
            ["Input.SourceOfTruthId"] = sourceOfTruthId.ToString("D")
        };

        if (ownerUserId.HasValue)
        {
            values["Input.OwnerUserId"] = ownerUserId.Value.ToString("D");
        }

        var response = await PostFormAsync(client, "/tenant/projects/new", values);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/tenant/projects", GetLocationPath(response));

        var listContent = await client.GetStringAsync("/tenant/projects");
        return ExtractFirstPath(listContent, "/tenant/projects/");
    }

    protected sealed record TenantContext(Guid TenantId, Guid SuperuserUserId, string SuperuserEmail, string SuperuserPassword);
}

public sealed class Epic3UserManagementAcceptanceTests : Epic3WorkflowAcceptanceTestBase
{
    [Fact]
    public async Task Tenant_users_list_shows_two_tenant_users_and_hides_different_tenant_users()
    {
        var tenant = await SeedTenantContextAsync("A Datum", "A Datum Owner", "adatum.owner.users@tenant.local", "Tenant0001!");
        await SeedTenantUserAsync(tenant.TenantId, "A Datum Hiring Manager", "adatum.hm.users@tenant.local", "Tenant0001!");

        var otherTenant = await SeedTenantContextAsync("Wingtip", "Wingtip Owner", "wingtip.owner.users@tenant.local", "Tenant0001!");
        await SeedTenantUserAsync(otherTenant.TenantId, "Wingtip Hiring Manager", "wingtip.hm.users@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var response = await client.GetAsync("/tenant/users");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, Regex.Matches(content, "data-testid=\"tenant-user-row\"", RegexOptions.CultureInvariant).Count);
        Assert.DoesNotContain("wingtip.hm.users@tenant.local", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_users_page_exposes_a_create_hiring_manager_action()
    {
        var tenant = await SeedTenantContextAsync("Southridge", "Southridge Owner", "southridge.owner.users@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var content = await client.GetStringAsync("/tenant/users");

        Assert.Contains("Create Hiring Manager", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/users/new\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_user_creation_page_shows_required_inputs_and_submit_action()
    {
        var tenant = await SeedTenantContextAsync("Coho", "Coho Owner", "coho.owner.users@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var response = await client.GetAsync("/tenant/users/new");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("name=\"Input.DisplayName\"", content, StringComparison.Ordinal);
        Assert.Contains("name=\"Input.Email\"", content, StringComparison.Ordinal);
        Assert.Contains("name=\"Input.Password\"", content, StringComparison.Ordinal);
        Assert.Contains("type=\"submit\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Valid_hiring_manager_creation_redirects_to_tenant_users_and_displays_the_created_row()
    {
        var tenant = await SeedTenantContextAsync("Lucerne", "Lucerne Owner", "lucerne.owner.users@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        await CreateHiringManagerThroughUiAsync(client, "Lucerne Hiring Manager", "lucerne.hm.users@tenant.local", "Tenant0001!");

        var listContent = await client.GetStringAsync("/tenant/users");
        Assert.Contains("lucerne.hm.users@tenant.local", listContent, StringComparison.Ordinal);
        Assert.Contains("Hiring Manager", listContent, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Invalid_hiring_manager_creation_remains_on_the_form_page()
    {
        var tenant = await SeedTenantContextAsync("Fourth Coffee", "Fourth Coffee Owner", "fourth.owner.users@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var response = await PostFormAsync(
            client,
            "/tenant/users/new",
            new Dictionary<string, string>
            {
                ["Input.DisplayName"] = string.Empty,
                ["Input.Email"] = "fourth.hm.users@tenant.local",
                ["Input.Password"] = "Tenant0001!"
            });

        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("name=\"Input.DisplayName\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Hiring_manager_created_through_tenant_users_can_sign_in_to_the_tenant_dashboard()
    {
        var tenant = await SeedTenantContextAsync("Consolidated", "Consolidated Owner", "consolidated.owner.users@tenant.local", "Tenant0001!");

        using var superuserClient = CreateClient();
        await SignInAsync(superuserClient, tenant.SuperuserEmail, tenant.SuperuserPassword);
        await CreateHiringManagerThroughUiAsync(superuserClient, "Consolidated Hiring Manager", "consolidated.hm.users@tenant.local", "Tenant0001!");

        using var hiringManagerClient = CreateClient();
        await SignInAsync(hiringManagerClient, "consolidated.hm.users@tenant.local", "Tenant0001!");

        var response = await hiringManagerClient.GetAsync("/tenant/dashboard");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("hiringmanager", "/tenant/users")]
    [InlineData("hiringmanager", "/tenant/users/new")]
    [InlineData("anonymous", "/tenant/users")]
    [InlineData("anonymous", "/tenant/users/new")]
    public async Task Tenant_user_routes_enforce_superuser_access(string actor, string path)
    {
        var tenant = await SeedTenantContextAsync("Graphic Design", "Graphic Design Owner", "graphic.owner.users@tenant.local", "Tenant0001!");
        await SeedTenantUserAsync(tenant.TenantId, "Graphic Hiring Manager", "graphic.hm.users@tenant.local", "Tenant0001!");

        using var client = CreateClient();

        if (string.Equals(actor, "hiringmanager", StringComparison.Ordinal))
        {
            await SignInAsync(client, "graphic.hm.users@tenant.local", "Tenant0001!");
        }

        var response = await client.GetAsync(path);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/sign-in", GetLocationPath(response), StringComparison.Ordinal);
    }

    private Task<ApplicationUser> SeedTenantUserAsync(Guid tenantId, string displayName, string email, string password)
    {
        return SeedHiringManagerDirectAsync(tenantId, displayName, email, password);
    }
}

public sealed class Epic3SourceOfTruthAcceptanceTests : Epic3WorkflowAcceptanceTestBase
{
    [Fact]
    public async Task Tenant_source_of_truth_list_shows_two_tenant_rows_and_hides_different_tenant_rows()
    {
        var tenant = await SeedTenantContextAsync("Roadhouse", "Roadhouse Owner", "roadhouse.owner.sot@tenant.local", "Tenant0001!");
        await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Interview guide", "v1");
        await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Assessment prompts", "v2");

        var otherTenant = await SeedTenantContextAsync("Trey Research", "Trey Owner", "trey.owner.sot@tenant.local", "Tenant0001!");
        await SeedSourceOfTruthDirectAsync(otherTenant.TenantId, "Operations", "Glossary", "v1");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var response = await client.GetAsync("/tenant/sots");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, Regex.Matches(content, "data-testid=\"tenant-source-of-truth-row\"", RegexOptions.CultureInvariant).Count);
        Assert.DoesNotContain("Glossary", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_source_of_truth_list_exposes_a_create_action()
    {
        var tenant = await SeedTenantContextAsync("Margies", "Margies Owner", "margies.owner.sot@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var content = await client.GetStringAsync("/tenant/sots");

        Assert.Contains("Create source of truth", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/sots/new\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_source_of_truth_creation_page_shows_required_fields_and_actions()
    {
        var tenant = await SeedTenantContextAsync("School of Fine Art", "Fine Art Owner", "fine.owner.sot@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var response = await client.GetAsync("/tenant/sots/new");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("name=\"Input.Topic\"", content, StringComparison.Ordinal);
        Assert.Contains("name=\"Input.Name\"", content, StringComparison.Ordinal);
        Assert.Contains("name=\"Input.SchemaVersion\"", content, StringComparison.Ordinal);
        Assert.Contains("name=\"Input.EntryKey\"", content, StringComparison.Ordinal);
        Assert.Contains("name=\"Input.EntryLabel\"", content, StringComparison.Ordinal);
        Assert.Contains("name=\"Input.EntryValueType\"", content, StringComparison.Ordinal);
        Assert.Contains("name=\"Input.EntryValue\"", content, StringComparison.Ordinal);
        Assert.Contains("Add entry", content, StringComparison.Ordinal);
        Assert.Contains("Save source of truth", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Add_entry_on_source_of_truth_creation_page_shows_the_submitted_entry_row()
    {
        var tenant = await SeedTenantContextAsync("Fabrikam Design", "Design Owner", "design.owner.sot@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var response = await PostFormAsync(
            client,
            "/tenant/sots/new?handler=AddEntry",
            "/tenant/sots/new",
            new Dictionary<string, string>
            {
                ["Input.Topic"] = "Hiring",
                ["Input.Name"] = "Interview rubric",
                ["Input.SchemaVersion"] = "v1",
                ["Input.EntryKey"] = "team-fit",
                ["Input.EntryLabel"] = "Team fit",
                ["Input.EntryValueType"] = "Text",
                ["Input.EntryValue"] = "High collaboration",
                ["Input.EntriesJson"] = string.Empty
            });

        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("team-fit", content, StringComparison.Ordinal);
        Assert.Contains("Team fit", content, StringComparison.Ordinal);
        Assert.Contains("Text", content, StringComparison.Ordinal);
        Assert.Contains("High collaboration", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Valid_source_of_truth_creation_redirects_to_the_list_and_displays_the_created_row()
    {
        var tenant = await SeedTenantContextAsync("Northwind Design", "Northwind Owner", "northwind.owner.sot@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        await CreateSourceOfTruthThroughUiAsync(client, "Hiring", "Interview rubric", "v1", "team-fit", "Team fit", "Text", "High collaboration");

        var listContent = await client.GetStringAsync("/tenant/sots");
        Assert.Contains("Hiring", listContent, StringComparison.Ordinal);
        Assert.Contains("Interview rubric", listContent, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Source_of_truth_list_exposes_a_view_action_that_opens_the_detail_route()
    {
        var tenant = await SeedTenantContextAsync("City Power", "City Power Owner", "city.owner.sot@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var detailPath = await CreateSourceOfTruthThroughUiAsync(client, "Hiring", "Assessment profile", "v2", "focus", "Focus", "Text", "Delivery");
        var detailResponse = await client.GetAsync(detailPath);
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
    }

    [Fact]
    public async Task Source_of_truth_detail_shows_saved_values_and_entries()
    {
        var tenant = await SeedTenantContextAsync("Adventure Labs", "Adventure Labs Owner", "labs.owner.sot@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var detailPath = await CreateSourceOfTruthThroughUiAsync(client, "Hiring", "Assessment profile", "v2", "focus", "Focus", "Text", "Delivery");
        var response = await client.GetAsync(detailPath);
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Hiring", content, StringComparison.Ordinal);
        Assert.Contains("Assessment profile", content, StringComparison.Ordinal);
        Assert.Contains("v2", content, StringComparison.Ordinal);
        Assert.Contains("focus", content, StringComparison.Ordinal);
        Assert.Contains("Focus", content, StringComparison.Ordinal);
        Assert.Contains("Delivery", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Invalid_source_of_truth_creation_remains_on_the_form_page()
    {
        var tenant = await SeedTenantContextAsync("Consolidated Art", "Consolidated Art Owner", "consolidated.owner.sot@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var response = await PostFormAsync(
            client,
            "/tenant/sots/new?handler=Save",
            "/tenant/sots/new",
            new Dictionary<string, string>
            {
                ["Input.Topic"] = string.Empty,
                ["Input.Name"] = "Interview rubric",
                ["Input.SchemaVersion"] = "v1",
                ["Input.EntriesJson"] = string.Empty
            });

        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("name=\"Input.Topic\"", content, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("hiringmanager", "/tenant/sots")]
    [InlineData("hiringmanager", "/tenant/sots/new")]
    [InlineData("anonymous", "/tenant/sots/new")]
    public async Task Source_of_truth_routes_enforce_superuser_access(string actor, string path)
    {
        var tenant = await SeedTenantContextAsync("Kappa Arts", "Kappa Owner", "kappa.owner.sot@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "Kappa Hiring Manager", "kappa.hm.sot@tenant.local", "Tenant0001!");

        using var client = CreateClient();

        if (string.Equals(actor, "hiringmanager", StringComparison.Ordinal))
        {
            await SignInAsync(client, "kappa.hm.sot@tenant.local", "Tenant0001!");
        }

        var response = await client.GetAsync(path);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/sign-in", GetLocationPath(response), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Source_of_truth_detail_shows_not_found_for_a_different_tenant_record()
    {
        var tenant = await SeedTenantContextAsync("Humongous", "Humongous Owner", "humongous.owner.sot@tenant.local", "Tenant0001!");
        var otherTenant = await SeedTenantContextAsync("Wide World", "Wide World Owner", "wide.owner.sot@tenant.local", "Tenant0001!");

        using var otherClient = CreateClient();
        await SignInAsync(otherClient, otherTenant.SuperuserEmail, otherTenant.SuperuserPassword);
        var otherDetailPath = await CreateSourceOfTruthThroughUiAsync(otherClient, "Operations", "Glossary", "v1", "term", "Term", "Text", "Value");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var response = await client.GetAsync(otherDetailPath);
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Source of truth not found", content, StringComparison.Ordinal);
    }
}

public sealed class Epic3ProjectAcceptanceTests : Epic3WorkflowAcceptanceTestBase
{
    [Fact]
    public async Task Tenant_project_list_shows_two_tenant_rows_and_hides_different_tenant_rows()
    {
        var tenant = await SeedTenantContextAsync("Northwind Projects", "Northwind Owner", "northwind.owner.projects@tenant.local", "Tenant0001!");
        var ownerA = await SeedHiringManagerDirectAsync(tenant.TenantId, "Northwind HM A", "northwind.hm.a.projects@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "Northwind HM B", "northwind.hm.b.projects@tenant.local", "Tenant0001!");
        var source = await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Interview guide", "v1");

        using var tenantClient = CreateClient();
        await SignInAsync(tenantClient, tenant.SuperuserEmail, tenant.SuperuserPassword);
        await CreateProjectThroughUiAsync(tenantClient, "Platform Engineer", "Engineering", source.Id.Value, ownerA.Id);
        await CreateProjectThroughUiAsync(tenantClient, "Data Analyst", "Operations", source.Id.Value, ownerA.Id);

        var otherTenant = await SeedTenantContextAsync("Wingtip Projects", "Wingtip Owner", "wingtip.owner.projects@tenant.local", "Tenant0001!");
        var otherOwner = await SeedHiringManagerDirectAsync(otherTenant.TenantId, "Wingtip HM", "wingtip.hm.projects@tenant.local", "Tenant0001!");
        var otherSource = await SeedSourceOfTruthDirectAsync(otherTenant.TenantId, "Hiring", "Playbook", "v1");

        using var otherClient = CreateClient();
        await SignInAsync(otherClient, otherTenant.SuperuserEmail, otherTenant.SuperuserPassword);
        await CreateProjectThroughUiAsync(otherClient, "Sales Lead", "Sales", otherSource.Id.Value, otherOwner.Id);

        var response = await tenantClient.GetAsync("/tenant/projects");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, Regex.Matches(content, "data-testid=\"tenant-project-row\"", RegexOptions.CultureInvariant).Count);
        Assert.DoesNotContain("Sales Lead", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Hiring_manager_project_list_shows_only_owned_projects()
    {
        var tenant = await SeedTenantContextAsync("Blue Yonder Projects", "Blue Yonder Owner", "blue.owner.projects@tenant.local", "Tenant0001!");
        var ownerA = await SeedHiringManagerDirectAsync(tenant.TenantId, "Blue HM A", "blue.hm.a.projects@tenant.local", "Tenant0001!");
        var ownerB = await SeedHiringManagerDirectAsync(tenant.TenantId, "Blue HM B", "blue.hm.b.projects@tenant.local", "Tenant0001!");
        var source = await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Guide", "v1");

        using var superuserClient = CreateClient();
        await SignInAsync(superuserClient, tenant.SuperuserEmail, tenant.SuperuserPassword);
        await CreateProjectThroughUiAsync(superuserClient, "Platform Engineer", "Engineering", source.Id.Value, ownerA.Id);
        await CreateProjectThroughUiAsync(superuserClient, "Finance Manager", "Finance", source.Id.Value, ownerB.Id);

        using var hiringManagerClient = CreateClient();
        await SignInAsync(hiringManagerClient, "blue.hm.a.projects@tenant.local", "Tenant0001!");

        var response = await hiringManagerClient.GetAsync("/tenant/projects");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Platform Engineer", content, StringComparison.Ordinal);
        Assert.DoesNotContain("Finance Manager", content, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("superuser", "/tenant/projects")]
    [InlineData("hiringmanager", "/tenant/projects")]
    public async Task Project_list_exposes_create_action_for_both_tenant_roles(string actor, string path)
    {
        var tenant = await SeedTenantContextAsync("Graphic Projects", "Graphic Owner", "graphic.owner.projects@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "Graphic HM", "graphic.hm.projects@tenant.local", "Tenant0001!");

        using var client = CreateClient();

        if (string.Equals(actor, "superuser", StringComparison.Ordinal))
        {
            await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);
        }
        else
        {
            await SignInAsync(client, "graphic.hm.projects@tenant.local", "Tenant0001!");
        }

        var content = await client.GetStringAsync(path);

        Assert.Contains("Create project", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/projects/new\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Project_creation_page_for_superuser_shows_owner_and_source_selection()
    {
        var tenant = await SeedTenantContextAsync("Litware Projects", "Litware Owner", "litware.owner.projects@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "Litware HM", "litware.hm.projects@tenant.local", "Tenant0001!");
        await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var response = await client.GetAsync("/tenant/projects/new");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("name=\"Input.JobTitle\"", content, StringComparison.Ordinal);
        Assert.Contains("name=\"Input.Department\"", content, StringComparison.Ordinal);
        Assert.Contains("name=\"Input.OwnerUserId\"", content, StringComparison.Ordinal);
        Assert.Contains("name=\"Input.SourceOfTruthId\"", content, StringComparison.Ordinal);
        Assert.Contains("type=\"submit\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Project_creation_page_for_superuser_shows_only_tenant_hiring_managers_in_owner_selection()
    {
        var tenant = await SeedTenantContextAsync("Aline Projects", "Aline Owner", "aline.owner.projects@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "Aline HM", "aline.hm.projects@tenant.local", "Tenant0001!");
        await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Guide", "v1");

        var otherTenant = await SeedTenantContextAsync("Trey Projects", "Trey Owner", "trey.owner.projects@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(otherTenant.TenantId, "Trey HM", "trey.hm.projects@tenant.local", "Tenant0001!");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var content = await client.GetStringAsync("/tenant/projects/new");

        Assert.Contains("aline.hm.projects@tenant.local", content, StringComparison.Ordinal);
        Assert.DoesNotContain("trey.hm.projects@tenant.local", content, StringComparison.Ordinal);
        Assert.DoesNotContain(tenant.SuperuserEmail, content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Project_creation_page_for_hiring_manager_hides_owner_selection_and_shows_source_selection()
    {
        var tenant = await SeedTenantContextAsync("Wide Projects", "Wide Owner", "wide.owner.projects@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "Wide HM", "wide.hm.projects@tenant.local", "Tenant0001!");
        await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, "wide.hm.projects@tenant.local", "Tenant0001!");

        var response = await client.GetAsync("/tenant/projects/new");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("name=\"Input.JobTitle\"", content, StringComparison.Ordinal);
        Assert.Contains("name=\"Input.Department\"", content, StringComparison.Ordinal);
        Assert.Contains("name=\"Input.SourceOfTruthId\"", content, StringComparison.Ordinal);
        Assert.DoesNotContain("name=\"Input.OwnerUserId\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Project_creation_page_shows_only_tenant_sources_of_truth_in_selection()
    {
        var tenant = await SeedTenantContextAsync("Source Projects", "Source Owner", "source.owner.projects@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "Source HM", "source.hm.projects@tenant.local", "Tenant0001!");
        await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Tenant Guide", "v1");

        var otherTenant = await SeedTenantContextAsync("Other Source Projects", "Other Source Owner", "othersource.owner.projects@tenant.local", "Tenant0001!");
        await SeedSourceOfTruthDirectAsync(otherTenant.TenantId, "Operations", "Other Guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var content = await client.GetStringAsync("/tenant/projects/new");

        Assert.Contains("Tenant Guide", content, StringComparison.Ordinal);
        Assert.DoesNotContain("Other Guide", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Valid_superuser_project_creation_redirects_and_shows_selected_owner()
    {
        var tenant = await SeedTenantContextAsync("Lucerne Projects", "Lucerne Owner", "lucerne.owner.projects@tenant.local", "Tenant0001!");
        var owner = await SeedHiringManagerDirectAsync(tenant.TenantId, "Lucerne HM", "lucerne.hm.projects@tenant.local", "Tenant0001!");
        var source = await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        await CreateProjectThroughUiAsync(client, "Platform Engineer", "Engineering", source.Id.Value, owner.Id);

        var listContent = await client.GetStringAsync("/tenant/projects");
        Assert.Contains("Platform Engineer", listContent, StringComparison.Ordinal);
        Assert.Contains("Lucerne HM", listContent, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Valid_hiring_manager_project_creation_redirects_and_shows_signed_in_owner()
    {
        var tenant = await SeedTenantContextAsync("Consolidated Projects", "Consolidated Owner", "consolidated.owner.projects@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "Consolidated HM", "consolidated.hm.projects@tenant.local", "Tenant0001!");
        var source = await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, "consolidated.hm.projects@tenant.local", "Tenant0001!");

        await CreateProjectThroughUiAsync(client, "Data Analyst", "Operations", source.Id.Value);

        var listContent = await client.GetStringAsync("/tenant/projects");
        Assert.Contains("Data Analyst", listContent, StringComparison.Ordinal);
        Assert.Contains("Consolidated HM", listContent, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Superuser_project_creation_without_selected_owner_remains_on_the_form_page()
    {
        var tenant = await SeedTenantContextAsync("Graphic Factory", "Graphic Factory Owner", "graphicfactory.owner.projects@tenant.local", "Tenant0001!");
        await SeedHiringManagerDirectAsync(tenant.TenantId, "Graphic Factory HM", "graphicfactory.hm.projects@tenant.local", "Tenant0001!");
        var source = await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var response = await PostFormAsync(
            client,
            "/tenant/projects/new",
            new Dictionary<string, string>
            {
                ["Input.JobTitle"] = "Platform Engineer",
                ["Input.Department"] = "Engineering",
                ["Input.SourceOfTruthId"] = source.Id.Value.ToString("D")
            });

        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("name=\"Input.JobTitle\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Invalid_project_creation_remains_on_the_form_page()
    {
        var tenant = await SeedTenantContextAsync("Adatum Projects", "Adatum Owner", "adatum.owner.projects@tenant.local", "Tenant0001!");
        var owner = await SeedHiringManagerDirectAsync(tenant.TenantId, "Adatum HM", "adatum.hm.projects@tenant.local", "Tenant0001!");
        var source = await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var response = await PostFormAsync(
            client,
            "/tenant/projects/new",
            new Dictionary<string, string>
            {
                ["Input.JobTitle"] = string.Empty,
                ["Input.Department"] = "Engineering",
                ["Input.OwnerUserId"] = owner.Id.ToString("D"),
                ["Input.SourceOfTruthId"] = source.Id.Value.ToString("D")
            });

        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("name=\"Input.JobTitle\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Project_list_exposes_a_view_action_that_opens_the_detail_route()
    {
        var tenant = await SeedTenantContextAsync("Detail Projects", "Detail Owner", "detail.owner.projects@tenant.local", "Tenant0001!");
        var owner = await SeedHiringManagerDirectAsync(tenant.TenantId, "Detail HM", "detail.hm.projects@tenant.local", "Tenant0001!");
        var source = await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var detailPath = await CreateProjectThroughUiAsync(client, "Platform Engineer", "Engineering", source.Id.Value, owner.Id);
        var detailResponse = await client.GetAsync(detailPath);

        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
    }

    [Fact]
    public async Task Project_detail_shows_title_department_owner_and_selected_source_of_truth_name()
    {
        var tenant = await SeedTenantContextAsync("Detail View Projects", "Detail View Owner", "detailview.owner.projects@tenant.local", "Tenant0001!");
        var owner = await SeedHiringManagerDirectAsync(tenant.TenantId, "Detail View HM", "detailview.hm.projects@tenant.local", "Tenant0001!");
        var source = await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Interview guide", "v1");

        using var client = CreateClient();
        await SignInAsync(client, tenant.SuperuserEmail, tenant.SuperuserPassword);

        var detailPath = await CreateProjectThroughUiAsync(client, "Platform Engineer", "Engineering", source.Id.Value, owner.Id);
        var response = await client.GetAsync(detailPath);
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Platform Engineer", content, StringComparison.Ordinal);
        Assert.Contains("Engineering", content, StringComparison.Ordinal);
        Assert.Contains("Detail View HM", content, StringComparison.Ordinal);
        Assert.Contains("Interview guide", content, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("anonymous-list")]
    [InlineData("anonymous-new")]
    [InlineData("cross-tenant")]
    [InlineData("non-owner")]
    public async Task Project_routes_apply_access_and_not_found_rules(string scenario)
    {
        var tenant = await SeedTenantContextAsync("Rules Projects", "Rules Owner", "rules.owner.projects@tenant.local", "Tenant0001!");
        var owner = await SeedHiringManagerDirectAsync(tenant.TenantId, "Rules HM", "rules.hm.projects@tenant.local", "Tenant0001!");
        var source = await SeedSourceOfTruthDirectAsync(tenant.TenantId, "Hiring", "Guide", "v1");

        using var ownerClient = CreateClient();
        await SignInAsync(ownerClient, tenant.SuperuserEmail, tenant.SuperuserPassword);
        var projectPath = await CreateProjectThroughUiAsync(ownerClient, "Platform Engineer", "Engineering", source.Id.Value, owner.Id);

        if (string.Equals(scenario, "anonymous-list", StringComparison.Ordinal))
        {
            using var client = CreateClient();
            var response = await client.GetAsync("/tenant/projects");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("/sign-in", GetLocationPath(response), StringComparison.Ordinal);
            return;
        }

        if (string.Equals(scenario, "anonymous-new", StringComparison.Ordinal))
        {
            using var client = CreateClient();
            var response = await client.GetAsync("/tenant/projects/new");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("/sign-in", GetLocationPath(response), StringComparison.Ordinal);
            return;
        }

        if (string.Equals(scenario, "cross-tenant", StringComparison.Ordinal))
        {
            var otherTenant = await SeedTenantContextAsync("Cross Projects", "Cross Owner", "cross.owner.projects@tenant.local", "Tenant0001!");
            using var client = CreateClient();
            await SignInAsync(client, otherTenant.SuperuserEmail, otherTenant.SuperuserPassword);
            var response = await client.GetAsync(projectPath);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Project not found", content, StringComparison.Ordinal);
            return;
        }

        await SeedHiringManagerDirectAsync(tenant.TenantId, "Rules HM B", "rules.hm.b.projects@tenant.local", "Tenant0001!");
        using var nonOwnerClient = CreateClient();
        await SignInAsync(nonOwnerClient, "rules.hm.b.projects@tenant.local", "Tenant0001!");
        var nonOwnerResponse = await nonOwnerClient.GetAsync(projectPath);
        var nonOwnerContent = await nonOwnerResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, nonOwnerResponse.StatusCode);
        Assert.Contains("Project not found", nonOwnerContent, StringComparison.Ordinal);
    }
}

public sealed class Epic3BootstrapFlowAcceptanceTests : Epic3WorkflowAcceptanceTestBase
{
    [Fact]
    public async Task Tenant_bootstrap_flow_allows_superuser_to_create_hiring_manager_source_of_truth_and_project_and_allows_created_hiring_manager_to_open_the_created_project()
    {
        var tenant = await SeedTenantContextAsync("Bootstrap", "Bootstrap Owner", "bootstrap.owner.flow@tenant.local", "Tenant0001!");

        using var superuserClient = CreateClient();
        await SignInAsync(superuserClient, tenant.SuperuserEmail, tenant.SuperuserPassword);

        await CreateHiringManagerThroughUiAsync(superuserClient, "Bootstrap Hiring Manager", "bootstrap.hm.flow@tenant.local", "Tenant0001!");
        var sourceDetailPath = await CreateSourceOfTruthThroughUiAsync(
            superuserClient,
            "Hiring",
            "Bootstrap guide",
            "v1",
            "key",
            "Label",
            "Text",
            "Value");

        var projectSourceId = Guid.Parse(Regex.Match(sourceDetailPath, "[0-9a-fA-F\\-]{36}", RegexOptions.CultureInvariant).Value);

        await using var scope = Factory.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var hiringManager = await userManager.FindByEmailAsync("bootstrap.hm.flow@tenant.local");
        Assert.NotNull(hiringManager);

        var projectDetailPath = await CreateProjectThroughUiAsync(
            superuserClient,
            "Bootstrap Project",
            "Engineering",
            projectSourceId,
            hiringManager!.Id);

        var projectDetailResponse = await superuserClient.GetAsync(projectDetailPath);
        var projectDetailContent = await projectDetailResponse.Content.ReadAsStringAsync();

        using var hiringManagerClient = CreateClient();
        await SignInAsync(hiringManagerClient, "bootstrap.hm.flow@tenant.local", "Tenant0001!");
        var hiringManagerProjects = await hiringManagerClient.GetStringAsync("/tenant/projects");
        var hiringManagerProjectResponse = await hiringManagerClient.GetAsync(projectDetailPath);

        Assert.Contains("href=\"/tenant/users\"", await superuserClient.GetStringAsync("/tenant/dashboard"), StringComparison.Ordinal);
        Assert.Contains("bootstrap.hm.flow@tenant.local", await superuserClient.GetStringAsync("/tenant/users"), StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/sots\"", await superuserClient.GetStringAsync("/tenant/dashboard"), StringComparison.Ordinal);
        Assert.Contains("Bootstrap guide", await superuserClient.GetStringAsync("/tenant/sots"), StringComparison.Ordinal);
        Assert.Contains("href=\"/tenant/projects\"", await superuserClient.GetStringAsync("/tenant/dashboard"), StringComparison.Ordinal);
        Assert.Contains("Bootstrap Project", projectDetailContent, StringComparison.Ordinal);
        Assert.Contains("Bootstrap Hiring Manager", projectDetailContent, StringComparison.Ordinal);
        Assert.Contains("Bootstrap guide", projectDetailContent, StringComparison.Ordinal);
        Assert.Equal(HttpStatusCode.OK, hiringManagerProjectResponse.StatusCode);
        Assert.Contains("Bootstrap Project", hiringManagerProjects, StringComparison.Ordinal);
    }
}
