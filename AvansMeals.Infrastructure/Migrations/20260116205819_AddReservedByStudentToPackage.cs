using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvansMeals.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReservedByStudentToPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReservedByStudentId",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservedByStudentId",
                table: "Packages");
        }
    }
}
