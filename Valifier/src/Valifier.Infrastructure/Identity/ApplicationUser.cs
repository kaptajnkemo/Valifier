using Microsoft.AspNetCore.Identity;

namespace Valifier.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser()
    {
        Id = Guid.NewGuid();
    }

    public string DisplayName { get; set; } = string.Empty;

    public Guid? TenantId { get; set; }
}
