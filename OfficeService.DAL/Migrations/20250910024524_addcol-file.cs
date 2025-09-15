using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addcolfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "file_type",
                schema: "public",
                table: "file",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "file_type",
                schema: "public",
                table: "file");
        }
    }
}
