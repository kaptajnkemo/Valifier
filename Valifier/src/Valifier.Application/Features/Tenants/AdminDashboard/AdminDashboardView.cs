namespace Valifier.Application.Features.Tenants.AdminDashboard;

public sealed record AdminDashboardView(
    int TotalTenants,
    IReadOnlyList<AdminDashboardTenantRow> Tenants);
