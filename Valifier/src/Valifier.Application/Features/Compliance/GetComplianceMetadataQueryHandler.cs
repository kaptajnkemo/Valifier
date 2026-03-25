namespace Valifier.Application.Features.Compliance;

public sealed class GetComplianceMetadataQueryHandler
{
    private readonly IComplianceMetadataReader _reader;

    public GetComplianceMetadataQueryHandler(IComplianceMetadataReader reader)
    {
        _reader = reader;
    }

    public Task<ComplianceMetadataView?> HandleAsync(
        GetComplianceMetadataQuery query,
        CancellationToken cancellationToken = default)
    {
        return _reader.GetAsync(query.RecordType, query.RecordId, cancellationToken);
    }
}
