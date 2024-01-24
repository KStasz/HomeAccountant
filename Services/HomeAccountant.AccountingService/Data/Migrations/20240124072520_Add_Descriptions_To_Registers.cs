using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAccountant.AccountingService.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Descriptions_To_Registers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Registers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Registers");
        }
    }
}
