namespace Valifier.Application.Features.Tenants.AdminDashboard;

public interface IAdminDashboardReader
{
    Task<AdminDashboardView> GetAsync(CancellationToken cancellationToken);
}
