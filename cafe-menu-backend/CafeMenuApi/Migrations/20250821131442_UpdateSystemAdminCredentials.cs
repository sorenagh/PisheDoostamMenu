using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CafeMenuApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSystemAdminCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Password", "Username" },
                values: new object[] { "SuperAdmin@2025!", "superadmin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Password", "Username" },
                values: new object[] { "admin123", "sysadmin" });
        }
    }
}
