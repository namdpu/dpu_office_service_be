using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addcolfileversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ref_id",
                schema: "public",
                table: "file_version",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ref_id",
                schema: "public",
                table: "file_version");
        }
    }
}
