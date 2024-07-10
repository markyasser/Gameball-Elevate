using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gameball_Elevate.Migrations
{
    /// <inheritdoc />
    public partial class CurrentBalanceBack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentBalance",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentBalance",
                table: "Transactions");
        }
    }
}
