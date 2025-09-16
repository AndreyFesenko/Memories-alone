using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel_2025_09_17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                schema: "identity",
                table: "AuditLogs");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                schema: "identity",
                newName: "auditlogs",
                newSchema: "identity");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "identity",
                table: "auditlogs",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                schema: "identity",
                table: "users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "normalizedusername",
                schema: "identity",
                table: "users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "normalizedemail",
                schema: "identity",
                table: "users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                schema: "identity",
                table: "users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                schema: "identity",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "normalizedname",
                schema: "identity",
                table: "roles",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                schema: "identity",
                table: "roles",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_auditlogs",
                schema: "identity",
                table: "auditlogs",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_users_normalizedemail",
                schema: "identity",
                table: "users",
                column: "normalizedemail");

            migrationBuilder.CreateIndex(
                name: "ix_users_normalizedusername",
                schema: "identity",
                table: "users",
                column: "normalizedusername",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_normalizedemail",
                schema: "identity",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_normalizedusername",
                schema: "identity",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_auditlogs",
                schema: "identity",
                table: "auditlogs");

            migrationBuilder.RenameTable(
                name: "auditlogs",
                schema: "identity",
                newName: "AuditLogs",
                newSchema: "identity");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "identity",
                table: "AuditLogs",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                schema: "identity",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "normalizedusername",
                schema: "identity",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "normalizedemail",
                schema: "identity",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                schema: "identity",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                schema: "identity",
                table: "users",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "normalizedname",
                schema: "identity",
                table: "roles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                schema: "identity",
                table: "roles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogs",
                schema: "identity",
                table: "AuditLogs",
                column: "Id");
        }
    }
}
