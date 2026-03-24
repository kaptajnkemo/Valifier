using Valifier.Domain.Identity;

namespace Valifier.Infrastructure.Identity;

public sealed class DomainIdentityMapper
{
    public User ToDomain(ApplicationUser user, IEnumerable<ApplicationRole> roles)
    {
        var memberships = roles
            .Select(role => new UserRole(new RoleId(role.Id), role.Name ?? string.Empty))
            .ToArray();

        return new User(new UserId(user.Id), user.DisplayName, user.Email ?? string.Empty, memberships);
    }
}
