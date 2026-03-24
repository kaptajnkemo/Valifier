using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Valifier.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Epic1TenantBootstrap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InitialSuperuserDisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InitialSuperuserEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    InitialSuperuserUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InitialSuperuserHasSignedIn = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Users");
        }
    }
}
