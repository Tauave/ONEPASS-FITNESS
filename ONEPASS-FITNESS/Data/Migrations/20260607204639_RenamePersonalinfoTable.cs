using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONEPASS_FITNESS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenamePersonalinfoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassBookings_Personalinfos_Personalinfoid",
                table: "ClassBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Personalinfos_AspNetUsers_IdentityUserId",
                table: "Personalinfos");

            migrationBuilder.DropForeignKey(
                name: "FK_Progress_Personalinfos_Personalinfoid",
                table: "Progress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Personalinfos",
                table: "Personalinfos");

            migrationBuilder.RenameTable(
                name: "Personalinfos",
                newName: "Personalinfo");

            migrationBuilder.RenameIndex(
                name: "IX_Personalinfos_IdentityUserId",
                table: "Personalinfo",
                newName: "IX_Personalinfo_IdentityUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Personalinfo",
                table: "Personalinfo",
                column: "PersonalinfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassBookings_Personalinfo_Personalinfoid",
                table: "ClassBookings",
                column: "Personalinfoid",
                principalTable: "Personalinfo",
                principalColumn: "PersonalinfoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Personalinfo_AspNetUsers_IdentityUserId",
                table: "Personalinfo",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Progress_Personalinfo_Personalinfoid",
                table: "Progress",
                column: "Personalinfoid",
                principalTable: "Personalinfo",
                principalColumn: "PersonalinfoId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassBookings_Personalinfo_Personalinfoid",
                table: "ClassBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Personalinfo_AspNetUsers_IdentityUserId",
                table: "Personalinfo");

            migrationBuilder.DropForeignKey(
                name: "FK_Progress_Personalinfo_Personalinfoid",
                table: "Progress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Personalinfo",
                table: "Personalinfo");

            migrationBuilder.RenameTable(
                name: "Personalinfo",
                newName: "Personalinfos");

            migrationBuilder.RenameIndex(
                name: "IX_Personalinfo_IdentityUserId",
                table: "Personalinfos",
                newName: "IX_Personalinfos_IdentityUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Personalinfos",
                table: "Personalinfos",
                column: "PersonalinfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassBookings_Personalinfos_Personalinfoid",
                table: "ClassBookings",
                column: "Personalinfoid",
                principalTable: "Personalinfos",
                principalColumn: "PersonalinfoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Personalinfos_AspNetUsers_IdentityUserId",
                table: "Personalinfos",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Progress_Personalinfos_Personalinfoid",
                table: "Progress",
                column: "Personalinfoid",
                principalTable: "Personalinfos",
                principalColumn: "PersonalinfoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
