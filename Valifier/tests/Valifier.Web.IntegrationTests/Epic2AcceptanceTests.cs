using System.Data.Common;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Valifier.Domain.Identity;
using Valifier.Domain.Tenancy;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Web.IntegrationTests;

public sealed class Epic2AcceptanceTests : IAsyncLifetime
{
    private readonly Epic1WebApplicationFactory _factory = new();

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return _factory.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task Audit_store_inspection_after_one_committed_create_operation_shows_one_transaction_audit_record()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ValifierDbContext>();

        dbContext.Tenants.Add(CreateTenant("Create Audit Tenant"));
        await dbContext.SaveChangesAsync();

        var auditCount = await CountRowsAsync(dbContext, "TransactionAuditRecords");

        Assert.Equal(1, auditCount);
    }

    [Fact]
    public async Task Audit_store_inspection_after_one_committed_update_operation_shows_one_transaction_audit_record()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ValifierDbContext>();

        var tenant = CreateTenant("Update Audit Tenant");
        dbContext.Tenants.Add(tenant);
        await dbContext.SaveChangesAsync();

        tenant.MarkInitialSuperuserSignedIn();
        await dbContext.SaveChangesAsync();

        var auditCount = await CountRowsAsync(dbContext, "TransactionAuditRecords");
        var latestAuditRecord = await ReadLatestAuditRecordAsync(dbContext);

        Assert.Equal(2, auditCount);
        Assert.Equal("Update", latestAuditRecord.OperationType);
    }

    [Fact]
    public async Task Audit_store_inspection_after_one_committed_delete_operation_shows_one_transaction_audit_record()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ValifierDbContext>();

        var tenant = CreateTenant("Delete Audit Tenant");
        dbContext.Tenants.Add(tenant);
        await dbContext.SaveChangesAsync();

        dbContext.Tenants.Remove(tenant);
        await dbContext.SaveChangesAsync();

        var auditCount = await CountRowsAsync(dbContext, "TransactionAuditRecords");
        var latestAuditRecord = await ReadLatestAuditRecordAsync(dbContext);

        Assert.Equal(2, auditCount);
        Assert.Equal("Delete", latestAuditRecord.OperationType);
    }

    [Fact]
    public async Task Audit_store_inspection_after_one_write_operation_that_does_not_commit_shows_no_new_transaction_audit_record()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ValifierDbContext>();

        await using (var transaction = await dbContext.Database.BeginTransactionAsync())
        {
            dbContext.Tenants.Add(CreateTenant("Rolled Back Tenant"));
            await dbContext.SaveChangesAsync();
            await transaction.RollbackAsync();
        }

        dbContext.ChangeTracker.Clear();

        var auditCount = await CountRowsAsync(dbContext, "TransactionAuditRecords");

        Assert.Equal(0, auditCount);
    }

    [Fact]
    public async Task Audit_store_inspection_of_one_transaction_audit_record_shows_required_audit_fields()
    {
        var createdTenant = CreateTenant("Field Audit Tenant");

        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ValifierDbContext>();

        dbContext.Tenants.Add(createdTenant);
        await dbContext.SaveChangesAsync();

        var auditRecord = await ReadLatestAuditRecordAsync(dbContext);

        Assert.NotEqual(Guid.Empty, auditRecord.TransactionIdentifier);
        Assert.Equal(TimeSpan.Zero, auditRecord.UtcCommitTimestamp.Offset);
        Assert.Equal("Create", auditRecord.OperationType);
        Assert.Equal("Tenant", auditRecord.RecordType);
        Assert.Equal(createdTenant.Id.Value.ToString("D"), auditRecord.RecordIdentifier);
        Assert.Equal("Anonymous", auditRecord.ActorIdentifier);
        Assert.Equal("Platform", auditRecord.TenantIdentifier);
    }

    [Fact]
    public async Task Submission_of_an_update_request_for_an_existing_transaction_audit_record_returns_a_rejection_result()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ValifierDbContext>();

        dbContext.Tenants.Add(CreateTenant("Immutable Audit Tenant"));
        await dbContext.SaveChangesAsync();

        var auditRecord = await ReadLatestAuditRecordAsync(dbContext);

        using var client = _factory.CreateClient(allowAutoRedirect: false);
        using var response = await client.PutAsync(
            $"/api/audit/records/{auditRecord.Id:D}",
            CreateJsonContent(
                """
                {
                  "operationType": "Update"
                }
                """));

        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("immutable", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Api_request_for_compliance_metadata_for_an_existing_governed_record_returns_one_data_category_code()
    {
        var tenantId = await SeedTenantAndGetIdentifierAsync("Metadata Category Tenant");
        using var client = _factory.CreateClient(allowAutoRedirect: false);

        using var response = await client.GetAsync($"/api/compliance/metadata/tenant/{tenantId:D}");
        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(document.RootElement.GetProperty("dataCategoryCode").GetString()));
    }

    [Fact]
    public async Task Api_request_for_compliance_metadata_for_an_existing_governed_record_returns_one_retention_rule_code()
    {
        var tenantId = await SeedTenantAndGetIdentifierAsync("Metadata Retention Tenant");
        using var client = _factory.CreateClient(allowAutoRedirect: false);

        using var response = await client.GetAsync($"/api/compliance/metadata/tenant/{tenantId:D}");
        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(document.RootElement.GetProperty("retentionRuleCode").GetString()));
    }

    [Fact]
    public async Task Api_request_for_compliance_metadata_for_an_existing_governed_record_returns_one_collection_timestamp()
    {
        var tenantId = await SeedTenantAndGetIdentifierAsync("Metadata Timestamp Tenant");
        using var client = _factory.CreateClient(allowAutoRedirect: false);

        using var response = await client.GetAsync($"/api/compliance/metadata/tenant/{tenantId:D}");
        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(document.RootElement.TryGetProperty("collectionTimestamp", out var timestampElement));
        Assert.False(string.IsNullOrWhiteSpace(timestampElement.GetString()));
    }

    [Fact]
    public async Task Api_request_for_compliance_metadata_for_an_existing_governed_record_returns_one_subject_scope_value()
    {
        var tenantId = await SeedTenantAndGetIdentifierAsync("Metadata Scope Tenant");
        using var client = _factory.CreateClient(allowAutoRedirect: false);

        using var response = await client.GetAsync($"/api/compliance/metadata/tenant/{tenantId:D}");
        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(document.RootElement.GetProperty("subjectScope").GetString()));
    }

    [Fact]
    public async Task Api_request_for_compliance_metadata_for_a_non_existent_governed_record_returns_record_not_found()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);

        using var response = await client.GetAsync($"/api/compliance/metadata/tenant/{Guid.NewGuid():D}");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Record not found", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Api_submission_of_a_valid_access_request_creates_one_request_record_with_status_received()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);

        using var response = await client.PostAsync(
            "/api/privacy-requests",
            CreateJsonContent(
                """
                {
                  "subjectIdentifier": "subject-access-1",
                  "requestType": "Access"
                }
                """));

        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);

        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ValifierDbContext>();
        var requestCount = await CountRowsAsync(dbContext, "PrivacyRequests");

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(1, requestCount);
        Assert.Equal("Received", document.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Api_submission_of_a_valid_erasure_request_creates_one_request_record_with_status_received()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);

        using var response = await client.PostAsync(
            "/api/privacy-requests",
            CreateJsonContent(
                """
                {
                  "subjectIdentifier": "subject-erasure-1",
                  "requestType": "Erasure"
                }
                """));

        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);

        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ValifierDbContext>();
        var requestCount = await CountRowsAsync(dbContext, "PrivacyRequests");

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(1, requestCount);
        Assert.Equal("Received", document.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Api_submission_of_a_privacy_request_with_a_missing_subject_identifier_returns_a_rejection_result()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);

        using var response = await client.PostAsync(
            "/api/privacy-requests",
            CreateJsonContent(
                """
                {
                  "subjectIdentifier": "",
                  "requestType": "Access"
                }
                """));

        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("subject identifier", content, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("Received")]
    [InlineData("InReview")]
    [InlineData("Completed")]
    [InlineData("Rejected")]
    public async Task Api_request_for_privacy_request_details_returns_one_of_the_supported_status_values(string status)
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);
        var requestIdentifier = await CreatePrivacyRequestAsync(client, "status-subject-1", "Access");

        if (!string.Equals(status, "Received", StringComparison.Ordinal))
        {
            using var updateResponse = await client.PutAsync(
                $"/api/privacy-requests/{requestIdentifier:D}/status",
                CreateJsonContent(
                    $$"""
                    {
                      "status": "{{status}}"
                    }
                    """));

            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        }

        using var response = await client.GetAsync($"/api/privacy-requests/{requestIdentifier:D}");
        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(status, document.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Api_request_for_privacy_request_details_returns_request_identifier_subject_identifier_request_type_submitted_timestamp_and_current_status()
    {
        using var client = _factory.CreateClient(allowAutoRedirect: false);
        var requestIdentifier = await CreatePrivacyRequestAsync(client, "details-subject-1", "Access");

        using var response = await client.GetAsync($"/api/privacy-requests/{requestIdentifier:D}");
        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(requestIdentifier.ToString("D"), document.RootElement.GetProperty("requestIdentifier").GetString());
        Assert.Equal("details-subject-1", document.RootElement.GetProperty("subjectIdentifier").GetString());
        Assert.Equal("Access", document.RootElement.GetProperty("requestType").GetString());
        Assert.False(string.IsNullOrWhiteSpace(document.RootElement.GetProperty("submittedTimestamp").GetString()));
        Assert.Equal("Received", document.RootElement.GetProperty("status").GetString());
    }

    private async Task<Guid> SeedTenantAndGetIdentifierAsync(string tenantName)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ValifierDbContext>();

        var tenant = CreateTenant(tenantName);
        dbContext.Tenants.Add(tenant);
        await dbContext.SaveChangesAsync();

        return tenant.Id.Value;
    }

    private static Tenant CreateTenant(string tenantName)
    {
        return new Tenant(
            TenantId.New(),
            tenantName,
            $"{tenantName} Owner",
            $"{Guid.NewGuid():N}@tenant.local",
            new UserId(Guid.NewGuid()));
    }

    private static async Task<int> CountRowsAsync(ValifierDbContext dbContext, string tableName)
    {
        var connection = dbContext.Database.GetDbConnection();
        await EnsureOpenAsync(connection);

        await using var command = connection.CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM [{tableName}]";

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    private static async Task<TransactionAuditRecordRow> ReadLatestAuditRecordAsync(ValifierDbContext dbContext)
    {
        var connection = dbContext.Database.GetDbConnection();
        await EnsureOpenAsync(connection);

        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT TOP (1)
                [Id],
                [TransactionIdentifier],
                [UtcCommitTimestamp],
                [OperationType],
                [RecordType],
                [RecordIdentifier],
                [ActorIdentifier],
                [TenantIdentifier]
            FROM [TransactionAuditRecords]
            ORDER BY [UtcCommitTimestamp] DESC, [Id] DESC
            """;

        await using var reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync(), "Expected one transaction audit record.");

        return new TransactionAuditRecordRow(
            reader.GetGuid(0),
            reader.GetGuid(1),
            reader.GetFieldValue<DateTimeOffset>(2),
            reader.GetString(3),
            reader.GetString(4),
            reader.GetString(5),
            reader.GetString(6),
            reader.GetString(7));
    }

    private static async Task EnsureOpenAsync(DbConnection connection)
    {
        if (connection.State == System.Data.ConnectionState.Open)
        {
            return;
        }

        await connection.OpenAsync();
    }

    private static StringContent CreateJsonContent(string json)
    {
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static async Task<Guid> CreatePrivacyRequestAsync(HttpClient client, string subjectIdentifier, string requestType)
    {
        using var response = await client.PostAsync(
            "/api/privacy-requests",
            CreateJsonContent(
                $$"""
                {
                  "subjectIdentifier": "{{subjectIdentifier}}",
                  "requestType": "{{requestType}}"
                }
                """));

        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using var document = JsonDocument.Parse(content);
        return Guid.Parse(document.RootElement.GetProperty("requestIdentifier").GetString()!);
    }

    private sealed record TransactionAuditRecordRow(
        Guid Id,
        Guid TransactionIdentifier,
        DateTimeOffset UtcCommitTimestamp,
        string OperationType,
        string RecordType,
        string RecordIdentifier,
        string ActorIdentifier,
        string TenantIdentifier);
}
