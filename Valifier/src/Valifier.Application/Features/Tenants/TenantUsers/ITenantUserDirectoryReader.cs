namespace Valifier.Application.Features.Tenants.TenantUsers;

public interface ITenantUserDirectoryReader
{
    Task<TenantUserDirectoryView?> GetAsync(Guid userId, CancellationToken cancellationToken);
}
