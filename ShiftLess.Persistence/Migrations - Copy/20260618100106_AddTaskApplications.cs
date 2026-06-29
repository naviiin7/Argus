using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftLess.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "TaskApplications",
                newName: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskApplications_TaskRequestId",
                table: "TaskApplications",
                column: "TaskRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskApplications_WorkerId",
                table: "TaskApplications",
                column: "WorkerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskApplications_TaskRequests_TaskRequestId",
                table: "TaskApplications",
                column: "TaskRequestId",
                principalTable: "TaskRequests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskApplications_Users_WorkerId",
                table: "TaskApplications",
                column: "WorkerId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskApplications_TaskRequests_TaskRequestId",
                table: "TaskApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskApplications_Users_WorkerId",
                table: "TaskApplications");

            migrationBuilder.DropIndex(
                name: "IX_TaskApplications_TaskRequestId",
                table: "TaskApplications");

            migrationBuilder.DropIndex(
                name: "IX_TaskApplications_WorkerId",
                table: "TaskApplications");

            migrationBuilder.RenameColumn(
                name: "WorkerId",
                table: "TaskApplications",
                newName: "ClientId");
        }
    }
}
