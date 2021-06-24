using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shopify.Migrations
{
    public partial class update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductDetails");

            migrationBuilder.DropTable(
                name: "ShippingDetails");

            migrationBuilder.RenameColumn(
                name: "Approved",
                table: "Carts",
                newName: "Payed");

            migrationBuilder.AddColumn<int>(
                name: "GovernrateId",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_GovernrateId",
                table: "Carts",
                column: "GovernrateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_PaymentId",
                table: "Carts",
                column: "PaymentId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Governorates_GovernrateId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Payments_PaymentId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_GovernrateId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_PaymentId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "GovernrateId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Carts");

            migrationBuilder.RenameColumn(
                name: "Payed",
                table: "Carts",
                newName: "Approved");

            migrationBuilder.CreateTable(
                name: "ProductDetails",
                columns: table => new
                {
                    ProductDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Isdeleted = table.Column<bool>(type: "bit", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDetails", x => x.ProductDetailId);
                    table.ForeignKey(
                        name: "FK_ProductDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingDetails",
                columns: table => new
                {
                    ShippingDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartsCartId = table.Column<int>(type: "int", nullable: true),
                    GovernorateId = table.Column<int>(type: "int", nullable: false),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    PurshaesCost = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingDetails", x => x.ShippingDetailId);
                    table.ForeignKey(
                        name: "FK_ShippingDetails_Carts_CartsCartId",
                        column: x => x.CartsCartId,
                        principalTable: "Carts",
                        principalColumn: "CartId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingDetails_Governorates_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governorates",
                        principalColumn: "GovernorateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingDetails_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "PaymentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_ProductId",
                table: "ProductDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingDetails_CartsCartId",
                table: "ShippingDetails",
                column: "CartsCartId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingDetails_GovernorateId",
                table: "ShippingDetails",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingDetails_PaymentId",
                table: "ShippingDetails",
                column: "PaymentId",
                unique: true);
        }
    }
}
