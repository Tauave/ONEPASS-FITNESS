using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONEPASS_FITNESS.Migrations
{
    /// <inheritdoc />
    public partial class Progress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Progress_AspNetUsers_appUserId",
                table: "Progress");

            migrationBuilder.RenameColumn(
                name: "appUserId",
                table: "Progress",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Progress_appUserId",
                table: "Progress",
                newName: "IX_Progress_AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Progress_AspNetUsers_AppUserId",
                table: "Progress",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Progress_AspNetUsers_AppUserId",
                table: "Progress");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "Progress",
                newName: "appUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Progress_AppUserId",
                table: "Progress",
                newName: "IX_Progress_appUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Progress_AspNetUsers_appUserId",
                table: "Progress",
                column: "appUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
