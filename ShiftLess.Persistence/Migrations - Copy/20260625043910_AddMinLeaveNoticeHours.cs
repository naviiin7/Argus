using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftLess.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMinLeaveNoticeHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LeaveNoticeHours",
                table: "TaskRequests",
                newName: "MinimumLeaveNoticeHours");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinimumLeaveNoticeHours",
                table: "TaskRequests",
                newName: "LeaveNoticeHours");
        }
    }
}
