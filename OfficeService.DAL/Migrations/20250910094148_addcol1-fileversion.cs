using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addcol1fileversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "version",
                schema: "public",
                table: "file_version",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "version",
                schema: "public",
                table: "file_version");
        }
    }
}
