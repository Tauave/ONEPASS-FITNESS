using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ONEPASS_FITNESS.Migrations
{
    /// <inheritdoc />
    public partial class cbmodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ClassSessions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ClassSessions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ClassSessions",
                keyColumn: "Id",
                keyValue: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ClassSessions",
                columns: new[] { "Id", "Capacity", "ClassTypeId", "StartTime" },
                values: new object[,]
                {
                    { 1, 12, 1, new DateTime(2026, 9, 22, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 15, 2, new DateTime(2026, 9, 22, 18, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 10, 3, new DateTime(2026, 9, 23, 7, 0, 0, 0, DateTimeKind.Utc) }
                });
        }
    }
}
