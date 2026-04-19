using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace Software_Design1.Migrations
{
    public partial class AddFlorists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flowers_Categories_CategoryId",
                table: "Flowers");
            migrationBuilder.DropForeignKey(
                name: "FK_Flowers_Suppliers_SupplierId",
                table: "Flowers");
            migrationBuilder.DropTable(
                name: "Suppliers");
            migrationBuilder.DropIndex(
                name: "IX_Flowers_SupplierId",
                table: "Flowers");
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Flowers");
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Flowers");
            migrationBuilder.DropColumn(
                name: "Season",
                table: "Flowers");
            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "Flowers");
            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Flowers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Flowers",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
            migrationBuilder.AddColumn<int>(
                name: "FloristId",
                table: "Flowers",
                type: "int",
                nullable: true);
            migrationBuilder.CreateTable(
                name: "Florists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Specialization = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Bio = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContactEmail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Florists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Florists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
            migrationBuilder.CreateIndex(
                name: "IX_Flowers_FloristId",
                table: "Flowers",
                column: "FloristId");
            migrationBuilder.CreateIndex(
                name: "IX_Florists_UserId",
                table: "Florists",
                column: "UserId");
            migrationBuilder.AddForeignKey(
                name: "FK_Flowers_Categories_CategoryId",
                table: "Flowers",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
            migrationBuilder.AddForeignKey(
                name: "FK_Flowers_Florists_FloristId",
                table: "Flowers",
                column: "FloristId",
                principalTable: "Florists",
                principalColumn: "Id");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flowers_Categories_CategoryId",
                table: "Flowers");
            migrationBuilder.DropForeignKey(
                name: "FK_Flowers_Florists_FloristId",
                table: "Flowers");
            migrationBuilder.DropTable(
                name: "Florists");
            migrationBuilder.DropIndex(
                name: "IX_Flowers_FloristId",
                table: "Flowers");
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Flowers");
            migrationBuilder.DropColumn(
                name: "FloristId",
                table: "Flowers");
            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Flowers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Flowers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Flowers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
            migrationBuilder.AddColumn<string>(
                name: "Season",
                table: "Flowers",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "Flowers",
                type: "int",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContactEmail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
            migrationBuilder.CreateIndex(
                name: "IX_Flowers_SupplierId",
                table: "Flowers",
                column: "SupplierId");
            migrationBuilder.AddForeignKey(
                name: "FK_Flowers_Categories_CategoryId",
                table: "Flowers",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Flowers_Suppliers_SupplierId",
                table: "Flowers",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
