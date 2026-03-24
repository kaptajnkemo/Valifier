namespace Valifier.Domain.Identity;

public sealed class User
{
    private readonly List<UserRole> _roles;

    public User(UserId id, string displayName, string email, IEnumerable<UserRole>? roles = null)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name is required.", nameof(displayName));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        Id = id;
        DisplayName = displayName.Trim();
        Email = email.Trim();
        _roles = roles?.ToList() ?? [];
    }

    public UserId Id { get; }

    public string DisplayName { get; }

    public string Email { get; }

    public IReadOnlyList<UserRole> Roles => _roles;

    public void AssignRole(Role role)
    {
        if (_roles.Any(existing => existing.RoleId == role.Id))
        {
            return;
        }

        _roles.Add(new UserRole(role.Id, role.Name));
    }
}
