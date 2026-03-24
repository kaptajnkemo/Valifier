namespace Valifier.Application.Features.Tenants.SignInTracking;

public interface IInitialTenantSignInRecorder
{
    Task RecordAsync(Guid userId, CancellationToken cancellationToken);
}
