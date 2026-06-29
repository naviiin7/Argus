using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftLess.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EndpointFinalised : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationDeadline",
                table: "TaskRequests");

            migrationBuilder.RenameColumn(
                name: "Deadline",
                table: "TaskRequests",
                newName: "StartTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "TaskRequests",
                newName: "Deadline");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApplicationDeadline",
                table: "TaskRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
