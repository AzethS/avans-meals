using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvansMeals.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCanteenLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CanteenId",
                table: "Packages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CanteenId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_CanteenId",
                table: "Packages",
                column: "CanteenId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CanteenId",
                table: "AspNetUsers",
                column: "CanteenId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Canteens_CanteenId",
                table: "AspNetUsers",
                column: "CanteenId",
                principalTable: "Canteens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Canteens_CanteenId",
                table: "Packages",
                column: "CanteenId",
                principalTable: "Canteens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Canteens_CanteenId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Canteens_CanteenId",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_CanteenId",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CanteenId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CanteenId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "CanteenId",
                table: "AspNetUsers");
        }
    }
}
