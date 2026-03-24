namespace Valifier.Application.Features.Tenants.TenantDetail;

public sealed record TenantDetailView(
    Guid TenantId,
    string Name,
    string InitialSuperuserDisplayName,
    string InitialSuperuserEmail,
    bool FirstSignInCompleted);
