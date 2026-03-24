using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Valifier.Infrastructure.Identity;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Web.IntegrationTests;

public sealed class Epic1WebApplicationFactory : WebApplicationFactory<Program>, IAsyncDisposable
{
    private readonly string _databaseName = $"ValifierEpic1Tests_{Guid.NewGuid():N}";

    public string AdminEmail => "admin@valifier.local";

    public string AdminPassword => "Admin0001!";

    public string AdminDisplayName => "Global Admin";

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.UseSetting("environment", "Development");
        builder.UseSetting("ConnectionStrings:DefaultConnection", $"Server=(localdb)\\MSSQLLocalDB;Database={_databaseName};Trusted_Connection=True;TrustServerCertificate=True;");
        builder.UseSetting("BootstrapIdentity:AdminEmail", AdminEmail);
        builder.UseSetting("BootstrapIdentity:AdminPassword", AdminPassword);
        builder.UseSetting("BootstrapIdentity:AdminDisplayName", AdminDisplayName);
        builder.ConfigureServices(services =>
        {
            services.AddScoped<ITenantTestSeeder, TenantTestSeeder>();
        });
    }

    public HttpClient CreateClient(bool allowAutoRedirect)
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = allowAutoRedirect,
            HandleCookies = true
        });
    }

    public async Task SeedTenantAsync(
        string tenantName,
        string superuserDisplayName,
        string superuserEmail,
        string superuserPassword,
        bool hasCompletedFirstSignIn,
        CancellationToken cancellationToken = default)
    {
        await using var scope = Services.CreateAsyncScope();
        var provisioner = scope.ServiceProvider.GetRequiredService<ITenantTestSeeder>();

        await provisioner.SeedAsync(
            tenantName,
            superuserDisplayName,
            superuserEmail,
            superuserPassword,
            hasCompletedFirstSignIn,
            cancellationToken);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ValifierDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        Dispose();
    }
}

public interface ITenantTestSeeder
{
    Task SeedAsync(
        string tenantName,
        string superuserDisplayName,
        string superuserEmail,
        string superuserPassword,
        bool hasCompletedFirstSignIn,
        CancellationToken cancellationToken);
}
