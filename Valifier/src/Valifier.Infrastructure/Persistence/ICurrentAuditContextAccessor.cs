namespace Valifier.Infrastructure.Persistence;

public interface ICurrentAuditContextAccessor
{
    string GetActorIdentifier();

    string GetTenantIdentifier();
}
