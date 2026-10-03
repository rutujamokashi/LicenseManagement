using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicenseManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLicenseStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Licenses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Licenses");
        }
    }
}
