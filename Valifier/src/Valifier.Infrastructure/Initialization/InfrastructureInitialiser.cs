using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Valifier.Domain.Identity;
using Valifier.Infrastructure.Identity;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Infrastructure.Initialization;

public static class InfrastructureInitialiser
{
    public static async Task InitialiseAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var scopedServices = scope.ServiceProvider;

        var dbContext = scopedServices.GetRequiredService<ValifierDbContext>();
        var roleManager = scopedServices.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scopedServices.GetRequiredService<UserManager<ApplicationUser>>();
        var bootstrapIdentityOptions = scopedServices.GetRequiredService<IOptions<BootstrapIdentityOptions>>().Value;

        await dbContext.Database.MigrateAsync(cancellationToken);

        foreach (var role in RoleCatalog.All)
        {
            if (await roleManager.RoleExistsAsync(role.Name))
            {
                continue;
            }

            var applicationRole = new ApplicationRole(role.Name)
            {
                Id = role.Id.Value,
                Description = role.Description
            };

            var result = await roleManager.CreateAsync(applicationRole);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Unable to seed role '{role.Name}': {errors}");
            }
        }

        if (string.IsNullOrWhiteSpace(bootstrapIdentityOptions.AdminEmail) ||
            string.IsNullOrWhiteSpace(bootstrapIdentityOptions.AdminPassword) ||
            string.IsNullOrWhiteSpace(bootstrapIdentityOptions.AdminDisplayName))
        {
            return;
        }

        var adminUser = await userManager.FindByEmailAsync(bootstrapIdentityOptions.AdminEmail);

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                DisplayName = bootstrapIdentityOptions.AdminDisplayName.Trim(),
                Email = bootstrapIdentityOptions.AdminEmail.Trim(),
                UserName = bootstrapIdentityOptions.AdminEmail.Trim()
            };

            var createUserResult = await userManager.CreateAsync(adminUser, bootstrapIdentityOptions.AdminPassword);

            if (!createUserResult.Succeeded)
            {
                var errors = string.Join("; ", createUserResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Unable to seed admin user '{bootstrapIdentityOptions.AdminEmail}': {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, RoleNames.Admin))
        {
            var addToRoleResult = await userManager.AddToRoleAsync(adminUser, RoleNames.Admin);

            if (!addToRoleResult.Succeeded)
            {
                var errors = string.Join("; ", addToRoleResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Unable to assign admin role to '{bootstrapIdentityOptions.AdminEmail}': {errors}");
            }
        }
    }
}
