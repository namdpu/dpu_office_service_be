using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class initdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "file",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    app_id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    current_version = table.Column<int>(type: "integer", nullable: false),
                    url = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    origin_url = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    document_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    callback_url = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_userId = table.Column<Guid>(type: "uuid", nullable: true),
                    created_dateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_userId = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_dateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "file_version",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version = table.Column<string>(type: "text", nullable: false),
                    system_version = table.Column<int>(type: "integer", nullable: false),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    url = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    changes_url = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    last_save = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    force_save_type = table.Column<int>(type: "integer", nullable: true),
                    histotry = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    users = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    actions = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_userId = table.Column<Guid>(type: "uuid", nullable: true),
                    created_dateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_userId = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_dateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file_version", x => x.id);
                    table.ForeignKey(
                        name: "FK_file_version_file_file_id",
                        column: x => x.file_id,
                        principalSchema: "public",
                        principalTable: "file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_file_version_file_id",
                schema: "public",
                table: "file_version",
                column: "file_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "file_version",
                schema: "public");

            migrationBuilder.DropTable(
                name: "file",
                schema: "public");
        }
    }
}
