using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAccountant.AccountingService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBillingPeriodIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillingPeriodId",
                table: "Entries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingPeriodId",
                table: "Entries");
        }
    }
}
