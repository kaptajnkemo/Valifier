using Microsoft.AspNetCore.Identity;
using Valifier.Application.Features.Tenants.TenantUsers;
using Valifier.Domain.Identity;
using Valifier.Infrastructure.Identity;

namespace Valifier.Infrastructure.Services;

public sealed class TenantUserCreationService : ITenantUserCreationService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public TenantUserCreationService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Guid> CreateHiringManagerAsync(CreateHiringManagerCommand command, CancellationToken cancellationToken)
    {
        var actor = await _userManager.FindByIdAsync(command.ActorUserId.ToString("D"));

        if (actor?.TenantId is null || !await _userManager.IsInRoleAsync(actor, RoleNames.Superuser))
        {
            throw new InvalidOperationException("Only tenant superusers can create Hiring Manager users.");
        }

        var user = new ApplicationUser
        {
            DisplayName = command.DisplayName.Trim(),
            Email = command.Email.Trim(),
            UserName = command.Email.Trim(),
            TenantId = actor.TenantId.Value
        };

        var createResult = await _userManager.CreateAsync(user, command.Password);
        EnsureSuccess(createResult, "create the Hiring Manager user");

        var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.HiringManager);
        EnsureSuccess(roleResult, "assign the Hiring Manager role");

        return user.Id;
    }

    private static void EnsureSuccess(IdentityResult result, string operation)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join("; ", result.Errors.Select(error => error.Description));
        throw new InvalidOperationException($"Unable to {operation}: {errors}");
    }
}
