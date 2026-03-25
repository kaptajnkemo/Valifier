using Valifier.Application.Features.Compliance;
using Microsoft.AspNetCore.Identity;
using MudBlazor.Services;
using Valifier.Application.Features.Dashboard;
using Valifier.Application.Features.PrivacyRequests;
using Valifier.Application.Features.Tenants.AdminDashboard;
using Valifier.Application.Features.Tenants.Provisioning;
using Valifier.Application.Features.Tenants.TenantDetail;
using Valifier.Application.Features.Tenants.TenantWorkspace;
using Valifier.Infrastructure.DependencyInjection;
using Valifier.Infrastructure.Identity;
using Valifier.Infrastructure.Initialization;
using Valifier.Web.Components;
using Valifier.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddAuthorizationBuilder();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<ISecurityStampValidator, SecurityStampValidator<ApplicationUser>>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/sign-in";
    options.AccessDeniedPath = "/sign-in";
});
builder.Services.AddMudServices();
builder.Services.AddScoped<GetComplianceMetadataQueryHandler>();
builder.Services.AddScoped<GetPlatformOverviewQueryHandler>();
builder.Services.AddScoped<CreatePrivacyRequestCommandHandler>();
builder.Services.AddScoped<GetPrivacyRequestQueryHandler>();
builder.Services.AddScoped<GetAdminDashboardQueryHandler>();
builder.Services.AddScoped<CreateTenantCommandHandler>();
builder.Services.AddScoped<GetTenantDetailQueryHandler>();
builder.Services.AddScoped<GetTenantWorkspaceQueryHandler>();
builder.Services.AddScoped<UpdatePrivacyRequestStatusCommandHandler>();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await InfrastructureInitialiser.InitialiseAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorPages();
app.MapComplianceApi();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Valifier.Web.Client._Imports).Assembly);

app.Run();

public partial class Program;
