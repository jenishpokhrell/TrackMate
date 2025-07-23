using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace backend.Migrations
{
    public partial class initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "AccountGroups");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountTypeId",
                table: "AccountGroups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxAccounts = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountGroups_AccountTypeId",
                table: "AccountGroups",
                column: "AccountTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountGroups_AccountTypes_AccountTypeId",
                table: "AccountGroups",
                column: "AccountTypeId",
                principalTable: "AccountTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountGroups_AccountTypes_AccountTypeId",
                table: "AccountGroups");

            migrationBuilder.DropTable(
                name: "AccountTypes");

            migrationBuilder.DropIndex(
                name: "IX_AccountGroups_AccountTypeId",
                table: "AccountGroups");

            migrationBuilder.DropColumn(
                name: "AccountTypeId",
                table: "AccountGroups");

            migrationBuilder.AddColumn<int>(
                name: "AccountType",
                table: "AccountGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
