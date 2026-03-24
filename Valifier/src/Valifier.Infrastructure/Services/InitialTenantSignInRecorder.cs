using Microsoft.EntityFrameworkCore;
using Valifier.Domain.Identity;
using Valifier.Application.Features.Tenants.SignInTracking;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Infrastructure.Services;

public sealed class InitialTenantSignInRecorder : IInitialTenantSignInRecorder
{
    private readonly ValifierDbContext _dbContext;

    public InitialTenantSignInRecorder(ValifierDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task RecordAsync(Guid userId, CancellationToken cancellationToken)
    {
        var initialSuperuserUserId = new UserId(userId);
        var tenant = await _dbContext.Tenants
            .SingleOrDefaultAsync(candidate => candidate.InitialSuperuserUserId == initialSuperuserUserId, cancellationToken);

        if (tenant is null || tenant.InitialSuperuserHasSignedIn)
        {
            return;
        }

        tenant.MarkInitialSuperuserSignedIn();
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
