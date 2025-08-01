using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemoryArchiveService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitMemorySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "memory");

            migrationBuilder.CreateTable(
                name: "Memories",
                schema: "memory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AccessLevel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaFiles",
                schema: "memory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    MediaType = table.Column<int>(type: "integer", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OwnerId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    StorageUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaFiles_Memories_MemoryId",
                        column: x => x.MemoryId,
                        principalSchema: "memory",
                        principalTable: "Memories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "memory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MemoryId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Memories_MemoryId",
                        column: x => x.MemoryId,
                        principalSchema: "memory",
                        principalTable: "Memories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MemoryTags",
                schema: "memory",
                columns: table => new
                {
                    MemoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemoryTags", x => new { x.MemoryId, x.TagId });
                    table.ForeignKey(
                        name: "FK_MemoryTags_Memories_MemoryId",
                        column: x => x.MemoryId,
                        principalSchema: "memory",
                        principalTable: "Memories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemoryTags_Tags_TagId",
                        column: x => x.TagId,
                        principalSchema: "memory",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_MemoryId",
                schema: "memory",
                table: "MediaFiles",
                column: "MemoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MemoryTags_TagId",
                schema: "memory",
                table: "MemoryTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_MemoryId",
                schema: "memory",
                table: "Tags",
                column: "MemoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaFiles",
                schema: "memory");

            migrationBuilder.DropTable(
                name: "MemoryTags",
                schema: "memory");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "memory");

            migrationBuilder.DropTable(
                name: "Memories",
                schema: "memory");
        }
    }
}
