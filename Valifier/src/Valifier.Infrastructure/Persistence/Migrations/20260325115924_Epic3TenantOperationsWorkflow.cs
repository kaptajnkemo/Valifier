using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Valifier.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Epic3TenantOperationsWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerUserId",
                table: "RecruitmentProjects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SourceOfTruthId",
                table: "RecruitmentProjects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "TenantSourceOfTruthEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceOfTruthId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ValueType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantSourceOfTruthEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantSourceOfTruthEntries_TenantSourceOfTruths_SourceOfTruthId",
                        column: x => x.SourceOfTruthId,
                        principalTable: "TenantSourceOfTruths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentProjects_OwnerUserId",
                table: "RecruitmentProjects",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentProjects_SourceOfTruthId",
                table: "RecruitmentProjects",
                column: "SourceOfTruthId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantSourceOfTruthEntries_SourceOfTruthId",
                table: "TenantSourceOfTruthEntries",
                column: "SourceOfTruthId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantSourceOfTruthEntries");

            migrationBuilder.DropIndex(
                name: "IX_RecruitmentProjects_OwnerUserId",
                table: "RecruitmentProjects");

            migrationBuilder.DropIndex(
                name: "IX_RecruitmentProjects_SourceOfTruthId",
                table: "RecruitmentProjects");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "RecruitmentProjects");

            migrationBuilder.DropColumn(
                name: "SourceOfTruthId",
                table: "RecruitmentProjects");
        }
    }
}
