using System.Net;
using System.Text.RegularExpressions;

namespace Valifier.Web.IntegrationTests;

public sealed class Epic1AcceptanceTests : IAsyncLifetime
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
    public async Task Home_page_shows_a_sign_in_action()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);

        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Sign in", content, StringComparison.Ordinal);
        Assert.Contains("href=\"/sign-in\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Sign_in_page_shows_email_password_and_submit()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);

        var response = await client.GetAsync("/sign-in");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("name=\"Input.Email\"", content, StringComparison.Ordinal);
        Assert.Contains("name=\"Input.Password\"", content, StringComparison.Ordinal);
        Assert.Contains("type=\"submit\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Valid_global_admin_credentials_redirect_to_the_admin_dashboard()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);

        var response = await PostFormAsync(
            client,
            "/sign-in",
            new Dictionary<string, string>
            {
                ["Input.Email"] = _factory.AdminEmail,
                ["Input.Password"] = _factory.AdminPassword
            });

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/admin/dashboard", GetLocationPath(response));
    }

    [Fact]
    public async Task Admin_dashboard_requires_an_authenticated_global_admin_session()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);

        var response = await client.GetAsync("/admin/dashboard");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/sign-in", GetLocationPath(response), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Admin_dashboard_shows_the_total_tenant_count_for_three_tenants()
    {
        await _factory.SeedTenantAsync("Northwind", "Northwind Owner", "northwind.owner@tenant.local", "Tenant0001!", false);
        await _factory.SeedTenantAsync("Fabrikam", "Fabrikam Owner", "fabrikam.owner@tenant.local", "Tenant0001!", false);
        await _factory.SeedTenantAsync("Contoso", "Contoso Owner", "contoso.owner@tenant.local", "Tenant0001!", false);

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, _factory.AdminEmail, _factory.AdminPassword);

        var response = await client.GetAsync("/admin/dashboard");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Total tenants", content, StringComparison.Ordinal);
        Assert.Contains("data-testid=\"tenant-count-value\">3<", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Admin_dashboard_shows_one_row_per_tenant_record()
    {
        await _factory.SeedTenantAsync("Northwind", "Northwind Owner", "northwind.owner2@tenant.local", "Tenant0001!", false);
        await _factory.SeedTenantAsync("Fabrikam", "Fabrikam Owner", "fabrikam.owner2@tenant.local", "Tenant0001!", false);
        await _factory.SeedTenantAsync("Contoso", "Contoso Owner", "contoso.owner2@tenant.local", "Tenant0001!", false);

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, _factory.AdminEmail, _factory.AdminPassword);

        var response = await client.GetAsync("/admin/dashboard");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(3, Regex.Matches(content, "data-testid=\"tenant-row\"", RegexOptions.CultureInvariant).Count);
    }

    [Fact]
    public async Task Admin_dashboard_shows_no_tenants_found_when_zero_tenants_exist()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, _factory.AdminEmail, _factory.AdminPassword);

        var response = await client.GetAsync("/admin/dashboard");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("No tenants found", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Admin_dashboard_exposes_a_create_tenant_action()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, _factory.AdminEmail, _factory.AdminPassword);

        var response = await client.GetAsync("/admin/dashboard");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("href=\"/admin/tenants/new\"", content, StringComparison.Ordinal);
        Assert.Contains("Create tenant", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Admin_dashboard_exposes_a_view_tenant_action_for_each_row()
    {
        await _factory.SeedTenantAsync("Northwind", "Northwind Owner", "northwind.owner3@tenant.local", "Tenant0001!", false);

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, _factory.AdminEmail, _factory.AdminPassword);

        var response = await client.GetAsync("/admin/dashboard");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("View tenant", content, StringComparison.Ordinal);
        Assert.Contains("/admin/tenants/", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_creation_page_shows_the_required_inputs_and_submit_action()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, _factory.AdminEmail, _factory.AdminPassword);

        var response = await client.GetAsync("/admin/tenants/new");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("name=\"Input.TenantName\"", content, StringComparison.Ordinal);
        Assert.Contains("name=\"Input.InitialSuperuserDisplayName\"", content, StringComparison.Ordinal);
        Assert.Contains("name=\"Input.InitialSuperuserEmail\"", content, StringComparison.Ordinal);
        Assert.Contains("name=\"Input.InitialSuperuserPassword\"", content, StringComparison.Ordinal);
        Assert.Contains("type=\"submit\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Valid_tenant_creation_redirects_back_to_the_admin_dashboard()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, _factory.AdminEmail, _factory.AdminPassword);

        var response = await PostFormAsync(
            client,
            "/admin/tenants/new",
            new Dictionary<string, string>
            {
                ["Input.TenantName"] = "Blue Yonder",
                ["Input.InitialSuperuserDisplayName"] = "Blue Owner",
                ["Input.InitialSuperuserEmail"] = "blue.owner@tenant.local",
                ["Input.InitialSuperuserPassword"] = "Tenant0001!"
            });

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/admin/dashboard", GetLocationPath(response));
    }

    [Fact]
    public async Task Valid_tenant_creation_increases_the_dashboard_count_and_adds_the_row()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, _factory.AdminEmail, _factory.AdminPassword);

        await PostFormAsync(
            client,
            "/admin/tenants/new",
            new Dictionary<string, string>
            {
                ["Input.TenantName"] = "Blue Yonder",
                ["Input.InitialSuperuserDisplayName"] = "Blue Owner",
                ["Input.InitialSuperuserEmail"] = "blue.owner2@tenant.local",
                ["Input.InitialSuperuserPassword"] = "Tenant0001!"
            });

        var response = await client.GetAsync("/admin/dashboard");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("data-testid=\"tenant-count-value\">1<", content, StringComparison.Ordinal);
        Assert.Contains("Blue Yonder", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Invalid_tenant_creation_remains_on_the_form_page()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, _factory.AdminEmail, _factory.AdminPassword);

        var response = await PostFormAsync(
            client,
            "/admin/tenants/new",
            new Dictionary<string, string>
            {
                ["Input.TenantName"] = string.Empty,
                ["Input.InitialSuperuserDisplayName"] = "Blue Owner",
                ["Input.InitialSuperuserEmail"] = "blue.owner3@tenant.local",
                ["Input.InitialSuperuserPassword"] = "Tenant0001!"
            });

        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("name=\"Input.TenantName\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Invalid_tenant_creation_does_not_change_the_dashboard_count()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, _factory.AdminEmail, _factory.AdminPassword);

        await PostFormAsync(
            client,
            "/admin/tenants/new",
            new Dictionary<string, string>
            {
                ["Input.TenantName"] = string.Empty,
                ["Input.InitialSuperuserDisplayName"] = "Blue Owner",
                ["Input.InitialSuperuserEmail"] = "blue.owner4@tenant.local",
                ["Input.InitialSuperuserPassword"] = "Tenant0001!"
            });

        var response = await client.GetAsync("/admin/dashboard");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("data-testid=\"tenant-count-value\">0<", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_creation_requires_an_authenticated_global_admin_session()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);

        var response = await client.GetAsync("/admin/tenants/new");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/sign-in", GetLocationPath(response), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Valid_initial_tenant_superuser_credentials_redirect_to_the_tenant_dashboard()
    {
        await _factory.SeedTenantAsync("Skyline", "Skyline Owner", "skyline.owner@tenant.local", "Tenant0001!", false);

        using var client = _factory.CreateClient(allowAutoRedirect: false);

        var response = await PostFormAsync(
            client,
            "/sign-in",
            new Dictionary<string, string>
            {
                ["Input.Email"] = "skyline.owner@tenant.local",
                ["Input.Password"] = "Tenant0001!"
            });

        var content = await response.Content.ReadAsStringAsync();

        Assert.True(response.StatusCode == HttpStatusCode.Redirect, content);
        Assert.Equal("/tenant/dashboard", GetLocationPath(response));
    }

    [Fact]
    public async Task Tenant_dashboard_shows_the_tenant_name_and_signed_in_user_email()
    {
        await _factory.SeedTenantAsync("Skyline", "Skyline Owner", "skyline.owner2@tenant.local", "Tenant0001!", false);

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, "skyline.owner2@tenant.local", "Tenant0001!");

        var response = await client.GetAsync("/tenant/dashboard");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Skyline", content, StringComparison.Ordinal);
        Assert.Contains("skyline.owner2@tenant.local", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_dashboard_requires_an_authenticated_tenant_superuser_session()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);

        var response = await client.GetAsync("/tenant/dashboard");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/sign-in", GetLocationPath(response), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Authenticated_tenant_superuser_is_redirected_away_from_the_admin_dashboard()
    {
        await _factory.SeedTenantAsync("Skyline", "Skyline Owner", "skyline.owner3@tenant.local", "Tenant0001!", false);

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, "skyline.owner3@tenant.local", "Tenant0001!");

        var response = await client.GetAsync("/admin/dashboard");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/sign-in", GetLocationPath(response), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_detail_page_shows_identity_values_and_no_first_sign_in_state()
    {
        await _factory.SeedTenantAsync("Northwind", "Northwind Owner", "northwind.owner4@tenant.local", "Tenant0001!", false);

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, _factory.AdminEmail, _factory.AdminPassword);

        var dashboard = await client.GetStringAsync("/admin/dashboard");
        var tenantPath = ExtractTenantDetailPath(dashboard);

        var detailResponse = await client.GetAsync(tenantPath);
        var detailContent = await detailResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
        Assert.Contains("Northwind", detailContent, StringComparison.Ordinal);
        Assert.Contains("Northwind Owner", detailContent, StringComparison.Ordinal);
        Assert.Contains("northwind.owner4@tenant.local", detailContent, StringComparison.Ordinal);
        Assert.Contains("First sign-in completed: No", detailContent, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_detail_page_shows_yes_after_the_initial_tenant_superuser_signs_in()
    {
        await _factory.SeedTenantAsync("Northwind", "Northwind Owner", "northwind.owner5@tenant.local", "Tenant0001!", false);

        using var tenantClient = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(tenantClient, "northwind.owner5@tenant.local", "Tenant0001!");

        using var adminClient = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(adminClient, _factory.AdminEmail, _factory.AdminPassword);

        var dashboard = await adminClient.GetStringAsync("/admin/dashboard");
        var tenantPath = ExtractTenantDetailPath(dashboard);

        var detailResponse = await adminClient.GetAsync(tenantPath);
        var detailContent = await detailResponse.Content.ReadAsStringAsync();

        Assert.Contains("First sign-in completed: Yes", detailContent, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_detail_page_shows_tenant_not_found_for_a_missing_record()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);
        await SignInAsync(client, _factory.AdminEmail, _factory.AdminPassword);

        var response = await client.GetAsync($"/admin/tenants/{Guid.NewGuid()}");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Tenant not found", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Tenant_detail_page_requires_an_authenticated_global_admin_session()
    {
        var responsePath = $"/admin/tenants/{Guid.NewGuid()}";
        using var client = _factory.CreateClient(allowAutoRedirect: false);

        var response = await client.GetAsync(responsePath);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/sign-in", GetLocationPath(response), StringComparison.Ordinal);
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
}
