using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace Software_Design1.Migrations
{
    public partial class AddAssignedFloristToOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
            migrationBuilder.AddColumn<int>(
                name: "AssignedFloristId",
                table: "Orders",
                type: "int",
                nullable: true);
            migrationBuilder.CreateIndex(
                name: "IX_Orders_AssignedFloristId",
                table: "Orders",
                column: "AssignedFloristId");
            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Florists_AssignedFloristId",
                table: "Orders",
                column: "AssignedFloristId",
                principalTable: "Florists",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Florists_AssignedFloristId",
                table: "Orders");
            migrationBuilder.DropIndex(
                name: "IX_Orders_AssignedFloristId",
                table: "Orders");
            migrationBuilder.DropColumn(
                name: "AssignedFloristId",
                table: "Orders");
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
