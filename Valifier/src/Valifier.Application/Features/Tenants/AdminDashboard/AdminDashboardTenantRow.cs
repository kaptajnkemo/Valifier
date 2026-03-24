namespace Valifier.Application.Features.Tenants.AdminDashboard;

public sealed record AdminDashboardTenantRow(
    Guid TenantId,
    string Name,
    string InitialSuperuserEmail,
    bool FirstSignInCompleted);
