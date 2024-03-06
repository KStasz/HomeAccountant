using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAccountant.FriendsService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueKeyToFriendshipNotificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SenderEmail",
                table: "FriendshipsNotifications",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "FriendshipsNotifications",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_FriendshipsNotifications_SenderEmail_OwnerId",
                table: "FriendshipsNotifications",
                columns: new[] { "SenderEmail", "OwnerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FriendshipsNotifications_SenderEmail_OwnerId",
                table: "FriendshipsNotifications");

            migrationBuilder.AlterColumn<string>(
                name: "SenderEmail",
                table: "FriendshipsNotifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "FriendshipsNotifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
