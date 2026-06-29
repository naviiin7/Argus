using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftLess.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationDeadlineAndLeaveNotice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinimumLeaveNoticeHours",
                table: "TaskRequests",
                newName: "LeaveNoticeHours");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApplicationDeadline",
                table: "TaskRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationDeadline",
                table: "TaskRequests");

            migrationBuilder.RenameColumn(
                name: "LeaveNoticeHours",
                table: "TaskRequests",
                newName: "MinimumLeaveNoticeHours");
        }
    }
}
