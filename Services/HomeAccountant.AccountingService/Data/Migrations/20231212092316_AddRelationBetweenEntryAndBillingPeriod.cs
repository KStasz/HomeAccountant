using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAccountant.AccountingService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationBetweenEntryAndBillingPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Entries_BillingPeriodId",
                table: "Entries",
                column: "BillingPeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_BillingPeriod_BillingPeriodId",
                table: "Entries",
                column: "BillingPeriodId",
                principalTable: "BillingPeriod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_BillingPeriod_BillingPeriodId",
                table: "Entries");

            migrationBuilder.DropIndex(
                name: "IX_Entries_BillingPeriodId",
                table: "Entries");
        }
    }
}
