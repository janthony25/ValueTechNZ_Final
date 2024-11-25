using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValueTechNZ_Final.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fixordermodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MyProperty",
                table: "Orders",
                newName: "OrderStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderStatus",
                table: "Orders",
                newName: "MyProperty");
        }
    }
}
