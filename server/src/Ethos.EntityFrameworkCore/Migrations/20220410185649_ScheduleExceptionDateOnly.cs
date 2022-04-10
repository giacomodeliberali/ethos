using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ethos.EntityFrameworkCore.Migrations
{
    public partial class ScheduleExceptionDateOnly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "Schedules",
                table: "Exceptions");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                schema: "Schedules",
                table: "Exceptions",
                newName: "Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                schema: "Schedules",
                table: "Exceptions",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                schema: "Schedules",
                table: "Exceptions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
