using Microsoft.EntityFrameworkCore.Migrations;

namespace Shopify.Migrations
{
    public partial class update2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_GovernrateId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_PaymentId",
                table: "Carts");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_GovernrateId",
                table: "Carts",
                column: "GovernrateId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_PaymentId",
                table: "Carts",
                column: "PaymentId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_GovernrateId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_PaymentId",
                table: "Carts");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_GovernrateId",
                table: "Carts",
                column: "GovernrateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_PaymentId",
                table: "Carts",
                column: "PaymentId");
        }
    }
}
