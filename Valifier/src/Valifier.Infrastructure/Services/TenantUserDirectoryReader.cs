using Microsoft.EntityFrameworkCore;
using Valifier.Application.Features.Tenants.TenantUsers;
using Valifier.Domain.Identity;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Infrastructure.Services;

public sealed class TenantUserDirectoryReader : ITenantUserDirectoryReader
{
    private readonly ValifierDbContext _dbContext;

    public TenantUserDirectoryReader(ValifierDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TenantUserDirectoryView?> GetAsync(Guid userId, CancellationToken cancellationToken)
    {
        var currentUserTenantId = await _dbContext.Users
            .Where(candidate => candidate.Id == userId && candidate.TenantId != null)
            .Select(candidate => candidate.TenantId)
            .SingleOrDefaultAsync(cancellationToken);

        if (!currentUserTenantId.HasValue)
        {
            return null;
        }

        var users = await (
            from user in _dbContext.Users
            join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
            join role in _dbContext.Roles on userRole.RoleId equals role.Id
            where user.TenantId == currentUserTenantId.Value
            orderby user.DisplayName, user.Email
            select new TenantUserDirectoryRow(
                user.DisplayName,
                user.Email ?? string.Empty,
                role.Name == RoleNames.HiringManager ? "Hiring Manager" : role.Name ?? string.Empty))
            .ToArrayAsync(cancellationToken);

        return new TenantUserDirectoryView(users.Length, users);
    }
}
