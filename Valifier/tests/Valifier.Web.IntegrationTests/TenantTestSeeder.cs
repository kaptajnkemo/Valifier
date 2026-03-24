using Microsoft.AspNetCore.Identity;
using Valifier.Application.Features.Tenants.Provisioning;
using Valifier.Application.Features.Tenants.SignInTracking;
using Valifier.Infrastructure.Identity;

namespace Valifier.Web.IntegrationTests;

public sealed class TenantTestSeeder : ITenantTestSeeder
{
    private readonly CreateTenantCommandHandler _createTenantCommandHandler;
    private readonly IInitialTenantSignInRecorder _initialTenantSignInRecorder;
    private readonly UserManager<ApplicationUser> _userManager;

    public TenantTestSeeder(
        CreateTenantCommandHandler createTenantCommandHandler,
        IInitialTenantSignInRecorder initialTenantSignInRecorder,
        UserManager<ApplicationUser> userManager)
    {
        _createTenantCommandHandler = createTenantCommandHandler;
        _initialTenantSignInRecorder = initialTenantSignInRecorder;
        _userManager = userManager;
    }

    public async Task SeedAsync(
        string tenantName,
        string superuserDisplayName,
        string superuserEmail,
        string superuserPassword,
        bool hasCompletedFirstSignIn,
        CancellationToken cancellationToken)
    {
        await _createTenantCommandHandler.HandleAsync(
            new CreateTenantCommand(
                tenantName,
                superuserDisplayName,
                superuserEmail,
                superuserPassword),
            cancellationToken);

        if (!hasCompletedFirstSignIn)
        {
            return;
        }

        var user = await _userManager.FindByEmailAsync(superuserEmail);

        if (user is not null)
        {
            await _initialTenantSignInRecorder.RecordAsync(user.Id, cancellationToken);
        }
    }
}
