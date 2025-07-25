using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessControlService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitAccessSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "access");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "AuditLogs",
                newSchema: "access");

            migrationBuilder.RenameTable(
                name: "AccessRules",
                newName: "AccessRules",
                newSchema: "access");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "AuditLogs",
                schema: "access",
                newName: "AuditLogs");

            migrationBuilder.RenameTable(
                name: "AccessRules",
                schema: "access",
                newName: "AccessRules");
        }
    }
}
