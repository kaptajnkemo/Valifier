namespace Valifier.Application.Features.Tenants.TenantUsers;

public sealed record CreateHiringManagerCommand(
    Guid ActorUserId,
    string DisplayName,
    string Email,
    string Password);
