using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAccountant.FriendsService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFriendshipIndex_IX_FriendId_UserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendshipsNotifications");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_FriendId_UserId",
                table: "Friendships",
                columns: new[] { "FriendId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Friendships_FriendId_UserId",
                table: "Friendships");

            migrationBuilder.CreateTable(
                name: "FriendshipsNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "1"),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SenderEmail = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendshipsNotifications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendshipsNotifications_SenderEmail_OwnerId",
                table: "FriendshipsNotifications",
                columns: new[] { "SenderEmail", "OwnerId" },
                unique: true);
        }
    }
}
