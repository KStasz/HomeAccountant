using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAccountant.FriendsService.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Friends",
                table: "Friends");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FriendRequests",
                table: "FriendRequests");

            migrationBuilder.RenameTable(
                name: "Friends",
                newName: "Friendships");

            migrationBuilder.RenameTable(
                name: "FriendRequests",
                newName: "FriendshipsNotifications");

            migrationBuilder.RenameIndex(
                name: "IX_Friends_UserId_FriendId",
                table: "Friendships",
                newName: "IX_Friendships_UserId_FriendId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Friendships",
                table: "Friendships",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FriendshipsNotifications",
                table: "FriendshipsNotifications",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FriendshipsNotifications",
                table: "FriendshipsNotifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Friendships",
                table: "Friendships");

            migrationBuilder.RenameTable(
                name: "FriendshipsNotifications",
                newName: "FriendRequests");

            migrationBuilder.RenameTable(
                name: "Friendships",
                newName: "Friends");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_UserId_FriendId",
                table: "Friends",
                newName: "IX_Friends_UserId_FriendId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FriendRequests",
                table: "FriendRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Friends",
                table: "Friends",
                column: "Id");
        }
    }
}
