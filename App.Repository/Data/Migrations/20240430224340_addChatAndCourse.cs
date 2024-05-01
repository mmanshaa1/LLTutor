using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class addChatAndCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChatContent",
                table: "Chats",
                newName: "History");

            migrationBuilder.AddColumn<int>(
                name: "ModelId",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Chats",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Chats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateTime",
                table: "Chats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Chats_AccountId",
                table: "Chats",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Accounts_AccountId",
                table: "Chats",
                column: "AccountId",
                principalSchema: "auth",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Accounts_AccountId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_AccountId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "LastUpdateTime",
                table: "Chats");

            migrationBuilder.RenameColumn(
                name: "History",
                table: "Chats",
                newName: "ChatContent");
        }
    }
}
