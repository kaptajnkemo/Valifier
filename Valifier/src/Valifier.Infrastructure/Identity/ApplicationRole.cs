using Microsoft.AspNetCore.Identity;

namespace Valifier.Infrastructure.Identity;

public sealed class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole()
    {
        Id = Guid.NewGuid();
    }

    public ApplicationRole(string name)
        : this()
    {
        Name = name;
        NormalizedName = name.ToUpperInvariant();
    }

    public string Description { get; set; } = string.Empty;
}
