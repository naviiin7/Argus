using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftLess.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameTaskFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequiredClients",
                table: "TaskRequests",
                newName: "ShopkeeperId");

            migrationBuilder.RenameColumn(
                name: "ManagerId",
                table: "TaskRequests",
                newName: "RequiredWorkers");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "TaskRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_TaskRequests_ShopkeeperId",
                table: "TaskRequests",
                column: "ShopkeeperId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskRequests_Users_ShopkeeperId",
                table: "TaskRequests",
                column: "ShopkeeperId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskRequests_Users_ShopkeeperId",
                table: "TaskRequests");

            migrationBuilder.DropIndex(
                name: "IX_TaskRequests_ShopkeeperId",
                table: "TaskRequests");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "TaskRequests");

            migrationBuilder.RenameColumn(
                name: "ShopkeeperId",
                table: "TaskRequests",
                newName: "RequiredClients");

            migrationBuilder.RenameColumn(
                name: "RequiredWorkers",
                table: "TaskRequests",
                newName: "ManagerId");
        }
    }
}
