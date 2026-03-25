using Microsoft.AspNetCore.Identity;
using Valifier.Application.Features.Tenants.TenantSourceOfTruths;
using Valifier.Domain.Identity;
using Valifier.Domain.Knowledge;
using Valifier.Domain.Tenancy;
using Valifier.Infrastructure.Identity;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Infrastructure.Services;

public sealed class TenantSourceOfTruthCreationService : ITenantSourceOfTruthCreationService
{
    private readonly ValifierDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public TenantSourceOfTruthCreationService(
        ValifierDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Guid> CreateAsync(CreateTenantSourceOfTruthCommand command, CancellationToken cancellationToken)
    {
        var actor = await _userManager.FindByIdAsync(command.ActorUserId.ToString("D"));

        if (actor?.TenantId is null || !await _userManager.IsInRoleAsync(actor, RoleNames.Superuser))
        {
            throw new InvalidOperationException("Only tenant superusers can create sources of truth.");
        }

        var sourceOfTruth = new TenantSourceOfTruth(
            TenantSourceOfTruthId.New(),
            new TenantId(actor.TenantId.Value),
            command.Topic,
            command.Name,
            command.SchemaVersion);

        foreach (var entry in command.Entries)
        {
            sourceOfTruth.AddEntry(
                new TenantSourceOfTruthEntry(
                    TenantSourceOfTruthEntryId.New(),
                    sourceOfTruth.Id,
                    entry.Key,
                    entry.Label,
                    entry.ValueType,
                    entry.Value));
        }

        _dbContext.TenantSourceOfTruths.Add(sourceOfTruth);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return sourceOfTruth.Id.Value;
    }
}
