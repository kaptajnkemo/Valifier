namespace Valifier.Application.Features.Tenants.TenantSourceOfTruths;

public interface ITenantSourceOfTruthDirectoryReader
{
    Task<TenantSourceOfTruthDirectoryView?> GetAsync(Guid userId, CancellationToken cancellationToken);
}
