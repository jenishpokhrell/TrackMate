using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace backend.Migrations
{
    public partial class initial3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_AccountGroups_AccountGroupId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_AccountGroupId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "AccountGroupId",
                table: "Categories");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountGroupId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Categories_AccountGroupId",
                table: "Categories",
                column: "AccountGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_AccountGroups_AccountGroupId",
                table: "Categories",
                column: "AccountGroupId",
                principalTable: "AccountGroups",
                principalColumn: "Id");
        }
    }
}
