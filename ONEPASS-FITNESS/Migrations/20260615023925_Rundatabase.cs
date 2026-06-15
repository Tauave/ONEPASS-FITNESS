using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONEPASS_FITNESS.Migrations
{
    /// <inheritdoc />
    public partial class Rundatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Personalinfoid",
                table: "Progress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Personalinfoid",
                table: "Progress",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
