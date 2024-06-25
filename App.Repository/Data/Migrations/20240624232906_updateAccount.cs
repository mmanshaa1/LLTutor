using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Country",
                schema: "auth",
                table: "Accounts",
                newName: "PhoneNumber2");

            migrationBuilder.AddColumn<bool>(
                name: "Gender",
                schema: "auth",
                table: "Accounts",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                schema: "auth",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                schema: "auth",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "dateOfBirth",
                schema: "auth",
                table: "Accounts",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                schema: "auth",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Location",
                schema: "auth",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Nationality",
                schema: "auth",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "dateOfBirth",
                schema: "auth",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber2",
                schema: "auth",
                table: "Accounts",
                newName: "Country");
        }
    }
}
