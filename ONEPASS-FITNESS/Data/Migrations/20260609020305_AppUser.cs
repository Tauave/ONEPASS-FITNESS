using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONEPASS_FITNESS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassBookings_Classes_Classid",
                table: "ClassBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassBookings_Personalinfo_Personalinfoid",
                table: "ClassBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Progress_Personalinfo_Personalinfoid",
                table: "Progress");

            migrationBuilder.DropTable(
                name: "Personalinfo");

            migrationBuilder.DropIndex(
                name: "IX_Progress_Personalinfoid",
                table: "Progress");

            migrationBuilder.DropIndex(
                name: "IX_ClassBookings_Personalinfoid",
                table: "ClassBookings");

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "Progress",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)",
                oldPrecision: 6,
                oldScale: 2);

            migrationBuilder.AddColumn<string>(
                name: "appUserId",
                table: "Progress",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "appUserId",
                table: "ClassBookings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DOB",
                table: "AspNetUsers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Lastname",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Progress_appUserId",
                table: "Progress",
                column: "appUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassBookings_appUserId",
                table: "ClassBookings",
                column: "appUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassBookings_AspNetUsers_appUserId",
                table: "ClassBookings",
                column: "appUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassBookings_Classes_Classid",
                table: "ClassBookings",
                column: "Classid",
                principalTable: "Classes",
                principalColumn: "Classid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Progress_AspNetUsers_appUserId",
                table: "Progress",
                column: "appUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassBookings_AspNetUsers_appUserId",
                table: "ClassBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassBookings_Classes_Classid",
                table: "ClassBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Progress_AspNetUsers_appUserId",
                table: "Progress");

            migrationBuilder.DropIndex(
                name: "IX_Progress_appUserId",
                table: "Progress");

            migrationBuilder.DropIndex(
                name: "IX_ClassBookings_appUserId",
                table: "ClassBookings");

            migrationBuilder.DropColumn(
                name: "appUserId",
                table: "Progress");

            migrationBuilder.DropColumn(
                name: "appUserId",
                table: "ClassBookings");

            migrationBuilder.DropColumn(
                name: "DOB",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Lastname",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "Progress",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateTable(
                name: "Personalinfo",
                columns: table => new
                {
                    PersonalinfoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DOB = table.Column<DateOnly>(type: "date", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentityUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personalinfo", x => x.PersonalinfoId);
                    table.ForeignKey(
                        name: "FK_Personalinfo_AspNetUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Progress_Personalinfoid",
                table: "Progress",
                column: "Personalinfoid");

            migrationBuilder.CreateIndex(
                name: "IX_ClassBookings_Personalinfoid",
                table: "ClassBookings",
                column: "Personalinfoid");

            migrationBuilder.CreateIndex(
                name: "IX_Personalinfo_IdentityUserId",
                table: "Personalinfo",
                column: "IdentityUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassBookings_Classes_Classid",
                table: "ClassBookings",
                column: "Classid",
                principalTable: "Classes",
                principalColumn: "Classid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassBookings_Personalinfo_Personalinfoid",
                table: "ClassBookings",
                column: "Personalinfoid",
                principalTable: "Personalinfo",
                principalColumn: "PersonalinfoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Progress_Personalinfo_Personalinfoid",
                table: "Progress",
                column: "Personalinfoid",
                principalTable: "Personalinfo",
                principalColumn: "PersonalinfoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
