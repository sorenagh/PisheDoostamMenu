using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CafeMenuApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlaceId",
                table: "MenuItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlaceId",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlaceId",
                table: "Admins",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoverImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "PlaceId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "PlaceId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "PlaceId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "PlaceId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "PlaceId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "PlaceId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "PlaceId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "PlaceId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "PlaceId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "PlaceId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "PlaceId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "PlaceId",
                value: 1);

            migrationBuilder.InsertData(
                table: "Places",
                columns: new[] { "Id", "Address", "CoverImage", "CreatedAt", "Description", "Email", "IsActive", "Logo", "Name", "Phone", "UpdatedAt" },
                values: new object[] { 1, "تهران، خیابان ولیعصر", "https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=800&h=400&fit=crop", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "کافه‌ای گرم و دوستانه با منوی متنوع", "info@pishedoostam.com", true, "https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=100&h=100&fit=crop", "کافه پیشه دوستان", "021-12345678", null });

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_PlaceId",
                table: "MenuItems",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_PlaceId",
                table: "Categories",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_PlaceId",
                table: "Admins",
                column: "PlaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_Places_PlaceId",
                table: "Admins",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Places_PlaceId",
                table: "Categories",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_Places_PlaceId",
                table: "MenuItems",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_Places_PlaceId",
                table: "Admins");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Places_PlaceId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_Places_PlaceId",
                table: "MenuItems");

            migrationBuilder.DropTable(
                name: "Places");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_PlaceId",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_Categories_PlaceId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Admins_PlaceId",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "Admins");
        }
    }
}
