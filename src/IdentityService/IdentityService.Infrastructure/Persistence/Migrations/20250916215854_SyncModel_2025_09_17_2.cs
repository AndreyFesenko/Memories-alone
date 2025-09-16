using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel_2025_09_17_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_roles_name",
                schema: "identity",
                table: "roles");

            migrationBuilder.CreateIndex(
                name: "IX_roles_normalizedname",
                schema: "identity",
                table: "roles",
                column: "normalizedname",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_roles_normalizedname",
                schema: "identity",
                table: "roles");

            migrationBuilder.CreateIndex(
                name: "IX_roles_name",
                schema: "identity",
                table: "roles",
                column: "name",
                unique: true);
        }
    }
}
