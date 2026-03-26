namespace Valifier.Application.Features.Tenants.TenantUsers;

public interface ITenantUserCreationService
{
    Task<Guid> CreateHiringManagerAsync(CreateHiringManagerCommand command, CancellationToken cancellationToken);
}
