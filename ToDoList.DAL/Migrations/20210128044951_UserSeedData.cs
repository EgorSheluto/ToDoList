using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDoList.API.Migrations
{
    public partial class UserSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreateDateUTC", "Login", "Password", "UpdateDateUTC" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "root", "root", null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreateDateUTC", "Login", "Password", "UpdateDateUTC" },
                values: new object[] { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "roots", "roots", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
