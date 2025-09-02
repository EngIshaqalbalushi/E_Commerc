using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_CommerceSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryAndSupplier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryCID",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Suppliers_SupplierSID",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryCID",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SupplierSID",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryCID",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SupplierSID",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CID",
                table: "Products",
                column: "CID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SID",
                table: "Products",
                column: "SID");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CID",
                table: "Products",
                column: "CID",
                principalTable: "Categories",
                principalColumn: "CID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Suppliers_SID",
                table: "Products",
                column: "SID",
                principalTable: "Suppliers",
                principalColumn: "SID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CID",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Suppliers_SID",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CID",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SID",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "CategoryCID",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SupplierSID",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryCID",
                table: "Products",
                column: "CategoryCID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierSID",
                table: "Products",
                column: "SupplierSID");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryCID",
                table: "Products",
                column: "CategoryCID",
                principalTable: "Categories",
                principalColumn: "CID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Suppliers_SupplierSID",
                table: "Products",
                column: "SupplierSID",
                principalTable: "Suppliers",
                principalColumn: "SID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
