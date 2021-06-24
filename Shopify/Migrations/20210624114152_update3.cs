using Microsoft.EntityFrameworkCore.Migrations;

namespace Shopify.Migrations
{
    public partial class update3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Governorates_GovernrateId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Payments_PaymentId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_PaymentId",
                table: "Carts");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "Carts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "GovernrateId",
                table: "Carts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_PaymentId",
                table: "Carts",
                column: "PaymentId",
                unique: true,
                filter: "[PaymentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Governorates_GovernrateId",
                table: "Carts",
                column: "GovernrateId",
                principalTable: "Governorates",
                principalColumn: "GovernorateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Payments_PaymentId",
                table: "Carts",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "PaymentId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Governorates_GovernrateId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Payments_PaymentId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_PaymentId",
                table: "Carts");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GovernrateId",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_PaymentId",
                table: "Carts",
                column: "PaymentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Governorates_GovernrateId",
                table: "Carts",
                column: "GovernrateId",
                principalTable: "Governorates",
                principalColumn: "GovernorateId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Payments_PaymentId",
                table: "Carts",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "PaymentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
