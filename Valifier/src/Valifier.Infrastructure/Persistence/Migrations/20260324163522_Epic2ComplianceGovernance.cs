using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Valifier.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Epic2ComplianceGovernance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CollectedAtUtc",
                table: "Tenants",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "PrivacyRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectIdentifier = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    SubmittedTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivacyRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionAuditRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionIdentifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UtcCommitTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    OperationType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    RecordType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    RecordIdentifier = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ActorIdentifier = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TenantIdentifier = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionAuditRecords", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivacyRequests");

            migrationBuilder.DropTable(
                name: "TransactionAuditRecords");

            migrationBuilder.DropColumn(
                name: "CollectedAtUtc",
                table: "Tenants");
        }
    }
}
