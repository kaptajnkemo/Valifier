using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Valifier.Application.Features.Dashboard;
using Valifier.Application.Features.Tenants.AdminDashboard;
using Valifier.Application.Features.Tenants.Provisioning;
using Valifier.Application.Features.Tenants.SignInTracking;
using Valifier.Application.Features.Tenants.TenantDetail;
using Valifier.Application.Features.Tenants.TenantWorkspace;
using Valifier.Infrastructure.Identity;
using Valifier.Infrastructure.Initialization;
using Valifier.Infrastructure.Persistence;
using Valifier.Infrastructure.Services;

namespace Valifier.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not configured.");

        services.AddDbContext<ValifierDbContext>(options => options.UseSqlServer(connectionString));

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequiredLength = 10;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ValifierDbContext>();

        services.Configure<BootstrapIdentityOptions>(configuration.GetSection("BootstrapIdentity"));
        services.AddScoped<IPlatformOverviewReader, PlatformOverviewReader>();
        services.AddScoped<IAdminDashboardReader, AdminDashboardReader>();
        services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();
        services.AddScoped<IInitialTenantSignInRecorder, InitialTenantSignInRecorder>();
        services.AddScoped<ITenantDetailReader, TenantDetailReader>();
        services.AddScoped<ITenantWorkspaceReader, TenantWorkspaceReader>();
        services.AddScoped<DomainIdentityMapper>();

        return services;
    }
}
