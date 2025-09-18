using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addcol2fileversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "synced",
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
                name: "synced",
                schema: "public",
                table: "file_version");
        }
    }
}
