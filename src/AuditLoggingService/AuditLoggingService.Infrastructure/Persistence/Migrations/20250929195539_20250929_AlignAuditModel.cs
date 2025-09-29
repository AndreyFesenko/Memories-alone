using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLoggingService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _20250929_AlignAuditModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "audit");

            // 1) почистить "UserId": всё, что не валидный UUID — в NULL
            migrationBuilder.Sql("""
        UPDATE audit."AuditLogs"
        SET "UserId" = NULL
        WHERE "UserId" IS NULL
           OR "UserId" = ''
           OR NOT "UserId" ~* '^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$';
        """);

            // 2) сменить тип с text/varchar на uuid через USING (без этого PG не сможет сконвертить)
            migrationBuilder.Sql("""
        ALTER TABLE audit."AuditLogs"
        ALTER COLUMN "UserId" TYPE uuid
        USING NULLIF("UserId",'')::uuid;
        """);

            // 3) индексы (если ещё не были)
            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                schema: "audit",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAt",
                schema: "audit",
                table: "AuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action",
                schema: "audit",
                table: "AuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                schema: "audit",
                table: "AuditLogs",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_AuditLogs_Timestamp", schema: "audit", table: "AuditLogs");
            migrationBuilder.DropIndex(name: "IX_AuditLogs_CreatedAt", schema: "audit", table: "AuditLogs");
            migrationBuilder.DropIndex(name: "IX_AuditLogs_Action", schema: "audit", table: "AuditLogs");
            migrationBuilder.DropIndex(name: "IX_AuditLogs_UserId", schema: "audit", table: "AuditLogs");

            // откатить обратно в text
            migrationBuilder.Sql("""
        ALTER TABLE audit."AuditLogs"
        ALTER COLUMN "UserId" TYPE text;
        """);
        }
    }
}
