using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLoggingService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitAuditSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "audit");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "AuditLogs",
                newSchema: "audit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "AuditLogs",
                schema: "audit",
                newName: "AuditLogs");
        }
    }
}
