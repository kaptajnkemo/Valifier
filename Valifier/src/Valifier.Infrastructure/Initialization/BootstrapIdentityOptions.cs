namespace Valifier.Infrastructure.Initialization;

public sealed class BootstrapIdentityOptions
{
    public string? AdminDisplayName { get; init; }

    public string? AdminEmail { get; init; }

    public string? AdminPassword { get; init; }
}
