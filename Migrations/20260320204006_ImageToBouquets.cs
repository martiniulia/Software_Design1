using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace Software_Design1.Migrations
{
    public partial class ImageToBouquets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Bouquets",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Bouquets");
        }
    }
}
