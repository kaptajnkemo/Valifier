namespace Valifier.Application.Features.Tenants.Provisioning;

public sealed record CreateTenantCommand(
    string TenantName,
    string InitialSuperuserDisplayName,
    string InitialSuperuserEmail,
    string InitialSuperuserPassword);
