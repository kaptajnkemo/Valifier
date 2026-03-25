namespace Valifier.Application.Features.Tenants.TenantUsers;

public sealed record TenantUserDirectoryView(
    int TotalUsers,
    IReadOnlyList<TenantUserDirectoryRow> Users);
