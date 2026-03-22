using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace Software_Design1.Migrations
{
    public partial class Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Flowers_FlowerId",
                table: "OrderItems");
            migrationBuilder.AlterColumn<int>(
                name: "FlowerId",
                table: "OrderItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
            migrationBuilder.AddColumn<int>(
                name: "BouquetId",
                table: "OrderItems",
                type: "int",
                nullable: true);
            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_BouquetId",
                table: "OrderItems",
                column: "BouquetId");
            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Bouquets_BouquetId",
                table: "OrderItems",
                column: "BouquetId",
                principalTable: "Bouquets",
                principalColumn: "Id");
            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Flowers_FlowerId",
                table: "OrderItems",
                column: "FlowerId",
                principalTable: "Flowers",
                principalColumn: "Id");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Bouquets_BouquetId",
                table: "OrderItems");
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Flowers_FlowerId",
                table: "OrderItems");
            migrationBuilder.DropIndex(
                name: "IX_OrderItems_BouquetId",
                table: "OrderItems");
            migrationBuilder.DropColumn(
                name: "BouquetId",
                table: "OrderItems");
            migrationBuilder.AlterColumn<int>(
                name: "FlowerId",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Flowers_FlowerId",
                table: "OrderItems",
                column: "FlowerId",
                principalTable: "Flowers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
