using System.Text.Json;

namespace Valifier.Web.IntegrationTests;

public sealed class Epic2ContractSchemaTests
{
    [Fact]
    public void Contract_test_for_the_transaction_audit_schema_fails_when_one_required_field_is_missing()
    {
        const string payload =
            """
            {
              "utcCommitTimestamp": "2026-03-24T16:00:00Z",
              "operationType": "Create",
              "recordType": "Tenant",
              "recordIdentifier": "9e39d22e-4e10-4557-acab-2bd4c51a43ba",
              "actorIdentifier": "Anonymous",
              "tenantIdentifier": "Platform"
            }
            """;

        var exception = Record.Exception(() => DeserializeContract("TransactionAuditContract", payload));

        Assert.IsType<JsonException>(exception);
    }

    [Fact]
    public void Contract_test_for_the_compliance_metadata_schema_fails_when_one_required_field_is_missing()
    {
        const string payload =
            """
            {
              "recordIdentifier": "9e39d22e-4e10-4557-acab-2bd4c51a43ba",
              "retentionRuleCode": "TENANT-ACCOUNT",
              "collectionTimestamp": "2026-03-24T16:00:00Z",
              "subjectScope": "Tenant"
            }
            """;

        var exception = Record.Exception(() => DeserializeContract("ComplianceMetadataContract", payload));

        Assert.IsType<JsonException>(exception);
    }

    [Fact]
    public void Contract_test_for_the_privacy_request_schema_fails_when_one_required_field_is_missing()
    {
        const string payload =
            """
            {
              "requestIdentifier": "9e39d22e-4e10-4557-acab-2bd4c51a43ba",
              "subjectIdentifier": "subject-42",
              "submittedTimestamp": "2026-03-24T16:00:00Z",
              "status": "Received"
            }
            """;

        var exception = Record.Exception(() => DeserializeContract("PrivacyRequestContract", payload));

        Assert.IsType<JsonException>(exception);
    }

    [Theory]
    [InlineData(
        "TransactionAuditContract",
        """
        {
          "transactionIdentifier": "9e39d22e-4e10-4557-acab-2bd4c51a43ba",
          "utcCommitTimestamp": "2026-03-24T16:00:00Z",
          "operationType": "Create",
          "recordType": "Tenant",
          "recordIdentifier": "9e39d22e-4e10-4557-acab-2bd4c51a43ba",
          "actorIdentifier": "Anonymous",
          "tenantIdentifier": "Platform"
        }
        """)]
    [InlineData(
        "ComplianceMetadataContract",
        """
        {
          "recordIdentifier": "9e39d22e-4e10-4557-acab-2bd4c51a43ba",
          "dataCategoryCode": "TENANT-PROFILE",
          "retentionRuleCode": "TENANT-ACCOUNT",
          "collectionTimestamp": "2026-03-24T16:00:00Z",
          "subjectScope": "Tenant"
        }
        """)]
    [InlineData(
        "PrivacyRequestContract",
        """
        {
          "requestIdentifier": "9e39d22e-4e10-4557-acab-2bd4c51a43ba",
          "subjectIdentifier": "subject-42",
          "requestType": "Access",
          "submittedTimestamp": "2026-03-24T16:00:00Z",
          "status": "Received"
        }
        """)]
    public void Contract_test_for_each_schema_passes_when_all_required_fields_are_present(string contractName, string payload)
    {
        var exception = Record.Exception(() => DeserializeContract(contractName, payload));

        Assert.Null(exception);
    }

    private static object? DeserializeContract(string contractName, string payload)
    {
        var contractType = Type.GetType($"Valifier.Contracts.{contractName}, Valifier.Contracts")
            ?? throw new InvalidOperationException($"Contract type '{contractName}' was not found.");

        return JsonSerializer.Deserialize(
            payload,
            contractType,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }
}
