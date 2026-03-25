namespace Valifier.Application.Features.Compliance;

public interface IComplianceMetadataReader
{
    Task<ComplianceMetadataView?> GetAsync(string recordType, Guid recordId, CancellationToken cancellationToken);
}
