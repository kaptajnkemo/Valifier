namespace Valifier.Application.Features.Tenants.TenantUsers;

public sealed record TenantUserDirectoryRow(
    string DisplayName,
    string Email,
    string RoleName);
