using Valifier.Application.Features.Compliance;
using Valifier.Application.Features.PrivacyRequests;
using Valifier.Contracts;

namespace Valifier.Web.Endpoints;

public static class ComplianceApiEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapComplianceApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            "/api/compliance/metadata/{recordType}/{recordId:guid}",
            async (
                string recordType,
                Guid recordId,
                GetComplianceMetadataQueryHandler handler,
                CancellationToken cancellationToken) =>
            {
                var view = await handler.HandleAsync(
                    new GetComplianceMetadataQuery(recordType, recordId),
                    cancellationToken);

                if (view is null)
                {
                    return Results.NotFound("Record not found");
                }

                return Results.Ok(
                    new ComplianceMetadataContract
                    {
                        RecordIdentifier = view.RecordIdentifier,
                        DataCategoryCode = view.DataCategoryCode,
                        RetentionRuleCode = view.RetentionRuleCode,
                        CollectionTimestamp = view.CollectionTimestamp,
                        SubjectScope = view.SubjectScope
                    });
            });

        endpoints.MapPut(
                "/api/audit/records/{auditRecordId:guid}",
                (Guid auditRecordId) => Results.BadRequest("Transaction audit records are immutable."))
            .DisableAntiforgery();

        endpoints.MapPost(
                "/api/privacy-requests",
                async (
                    CreatePrivacyRequestApiRequest request,
                    CreatePrivacyRequestCommandHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    try
                    {
                        var view = await handler.HandleAsync(
                            new CreatePrivacyRequestCommand(
                                request.SubjectIdentifier ?? string.Empty,
                                request.RequestType ?? string.Empty),
                            cancellationToken);

                        return Results.Created(
                            $"/api/privacy-requests/{view.RequestIdentifier:D}",
                            ToContract(view));
                    }
                    catch (ArgumentException exception)
                    {
                        return Results.BadRequest(exception.Message);
                    }
                })
            .DisableAntiforgery();

        endpoints.MapGet(
            "/api/privacy-requests/{requestId:guid}",
            async (
                Guid requestId,
                GetPrivacyRequestQueryHandler handler,
                CancellationToken cancellationToken) =>
            {
                var view = await handler.HandleAsync(
                    new GetPrivacyRequestQuery(requestId),
                    cancellationToken);

                return view is null ? Results.NotFound("Record not found") : Results.Ok(ToContract(view));
            });

        endpoints.MapPut(
                "/api/privacy-requests/{requestId:guid}/status",
                async (
                    Guid requestId,
                    UpdatePrivacyRequestStatusApiRequest request,
                    UpdatePrivacyRequestStatusCommandHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    try
                    {
                        var view = await handler.HandleAsync(
                            new UpdatePrivacyRequestStatusCommand(
                                requestId,
                                request.Status ?? string.Empty),
                            cancellationToken);

                        return view is null ? Results.NotFound("Record not found") : Results.Ok(ToContract(view));
                    }
                    catch (ArgumentException exception)
                    {
                        return Results.BadRequest(exception.Message);
                    }
                })
            .DisableAntiforgery();

        return endpoints;
    }

    private static PrivacyRequestContract ToContract(PrivacyRequestView view)
    {
        return new PrivacyRequestContract
        {
            RequestIdentifier = view.RequestIdentifier.ToString("D"),
            SubjectIdentifier = view.SubjectIdentifier,
            RequestType = view.RequestType,
            SubmittedTimestamp = view.SubmittedTimestamp,
            Status = view.Status
        };
    }

    private sealed class CreatePrivacyRequestApiRequest
    {
        public string? SubjectIdentifier { get; init; }

        public string? RequestType { get; init; }
    }

    private sealed class UpdatePrivacyRequestStatusApiRequest
    {
        public string? Status { get; init; }
    }
}
