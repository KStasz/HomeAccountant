using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAccountant.AccountingService.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExistingRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_Registers_RegisterId",
                table: "Entries");

            migrationBuilder.DropIndex(
                name: "IX_Entries_RegisterId",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "RegisterId",
                table: "Entries");

            migrationBuilder.CreateTable(
                name: "BillingPeriod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisterId = table.Column<int>(type: "int", nullable: false),
                    IsOpen = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "1"),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingPeriod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillingPeriod_Registers_RegisterId",
                        column: x => x.RegisterId,
                        principalTable: "Registers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillingPeriod_RegisterId",
                table: "BillingPeriod",
                column: "RegisterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillingPeriod");

            migrationBuilder.AddColumn<int>(
                name: "RegisterId",
                table: "Entries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Entries_RegisterId",
                table: "Entries",
                column: "RegisterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_Registers_RegisterId",
                table: "Entries",
                column: "RegisterId",
                principalTable: "Registers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
