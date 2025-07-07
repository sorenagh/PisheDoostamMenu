using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CafeMenuApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Photos = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "CreatedAt", "LastLoginAt", "Password", "Username" },
                values: new object[] { 1, new DateTime(2025, 6, 29, 16, 26, 13, 44, DateTimeKind.Utc).AddTicks(8263), null, "admin123", "admin" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Icon", "Name" },
                values: new object[,]
                {
                    { 1, "انواع قهوه‌های تازه و خوشمزه", "https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=100&h=100&fit=crop", "قهوه" },
                    { 2, "چای‌های معطر و گیاهی", "https://images.unsplash.com/photo-1544787219-7f47ccb76574?w=100&h=100&fit=crop", "چای" },
                    { 3, "نوشیدنی‌های خنک و طراوت‌بخش", "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?w=100&h=100&fit=crop", "نوشیدنی سرد" },
                    { 4, "دسرهای خوشمزه و شیرین", "https://images.unsplash.com/photo-1551024601-bec78aea704b?w=100&h=100&fit=crop", "دسر" },
                    { 5, "غذاهای اصلی و خوشمزه", "https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?w=100&h=100&fit=crop", "غذا" },
                    { 6, "صبحانه‌های مقوی و سالم", "https://images.unsplash.com/photo-1525351484163-7529414344d8?w=100&h=100&fit=crop", "صبحانه" }
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "CategoryId", "Description", "Image", "Name", "Photos", "Price" },
                values: new object[,]
                {
                    { 1, 1, "قهوه تلخ و قوی", "https://images.unsplash.com/photo-1510707577719-ae7c14805e3a?w=300&h=200&fit=crop", "اسپرسو", "https://images.unsplash.com/photo-1510707577719-ae7c14805e3a?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1447933601403-0c6688de566e?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1542181961-9590d0c79dab?w=300&h=200&fit=crop", 25000m },
                    { 2, 1, "قهوه با شیر و فوم", "https://images.unsplash.com/photo-1534778101976-62847782c213?w=300&h=200&fit=crop", "کاپوچینو", "https://images.unsplash.com/photo-1534778101976-62847782c213?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1461023058943-07fcbe16d735?w=300&h=200&fit=crop", 35000m },
                    { 3, 2, "چای سنتی ایرانی", "https://images.unsplash.com/photo-1544787219-7f47ccb76574?w=300&h=200&fit=crop", "چای سیاه", "https://images.unsplash.com/photo-1544787219-7f47ccb76574?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1576092768241-dec231879fc3?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1597318181409-cf85ba05cd49?w=300&h=200&fit=crop", 20000m },
                    { 4, 3, "قهوه سرد و خوشمزه", "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?w=300&h=200&fit=crop", "آیس کافی", "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1461023058943-07fcbe16d735?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1497515114629-f71d768fd07c?w=300&h=200&fit=crop", 40000m },
                    { 5, 4, "دسر ایتالیایی خوشمزه", "https://images.unsplash.com/photo-1551024601-bec78aea704b?w=300&h=200&fit=crop", "تیرامیسو", "https://images.unsplash.com/photo-1551024601-bec78aea704b?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1464349095431-e9a21285b5f3?w=300&h=200&fit=crop", 55000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_Username",
                table: "Admins",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_CategoryId",
                table: "MenuItems",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
