using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftLess.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaveNoticeHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LeaveNoticeHours",
                table: "TaskRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeaveNoticeHours",
                table: "TaskRequests");
        }
    }
}
