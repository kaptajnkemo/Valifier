using Microsoft.AspNetCore.Identity;
using Valifier.Application.Features.Tenants.Provisioning;
using Valifier.Domain.Identity;
using Valifier.Domain.Tenancy;
using Valifier.Infrastructure.Identity;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Infrastructure.Services;

public sealed class TenantProvisioningService : ITenantProvisioningService
{
    private readonly ValifierDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public TenantProvisioningService(
        ValifierDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Guid> CreateAsync(CreateTenantCommand command, CancellationToken cancellationToken)
    {
        var tenantId = TenantId.New();

        var user = new ApplicationUser
        {
            DisplayName = command.InitialSuperuserDisplayName.Trim(),
            Email = command.InitialSuperuserEmail.Trim(),
            TenantId = tenantId.Value,
            UserName = command.InitialSuperuserEmail.Trim()
        };

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var createUserResult = await _userManager.CreateAsync(user, command.InitialSuperuserPassword);
        EnsureSuccess(createUserResult, "create the initial tenant superuser");

        var addToRoleResult = await _userManager.AddToRoleAsync(user, RoleNames.Superuser);
        EnsureSuccess(addToRoleResult, "assign the initial tenant superuser role");

        var tenant = new Tenant(
            tenantId,
            command.TenantName.Trim(),
            command.InitialSuperuserDisplayName.Trim(),
            command.InitialSuperuserEmail.Trim(),
            new UserId(user.Id));

        _dbContext.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return tenant.Id.Value;
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
