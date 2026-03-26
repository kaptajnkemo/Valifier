using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Valifier.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Epic3TenantWorkspaceBootstrap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "RecruitmentProjects",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TenantSourceOfTruths",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SchemaVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantSourceOfTruths", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentProjects_TenantId",
                table: "RecruitmentProjects",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantSourceOfTruths_TenantId",
                table: "TenantSourceOfTruths",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantSourceOfTruths");

            migrationBuilder.DropIndex(
                name: "IX_RecruitmentProjects_TenantId",
                table: "RecruitmentProjects");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "RecruitmentProjects");
        }
    }
}
