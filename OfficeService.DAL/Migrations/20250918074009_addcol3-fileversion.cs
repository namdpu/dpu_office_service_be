using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addcol3fileversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "synced",
                schema: "public",
                table: "file_version",
                newName: "synced_file");

            migrationBuilder.AddColumn<bool>(
                name: "synced_changes",
                schema: "public",
                table: "file_version",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "synced_changes",
                schema: "public",
                table: "file_version");

            migrationBuilder.RenameColumn(
                name: "synced_file",
                schema: "public",
                table: "file_version",
                newName: "synced");
        }
    }
}
