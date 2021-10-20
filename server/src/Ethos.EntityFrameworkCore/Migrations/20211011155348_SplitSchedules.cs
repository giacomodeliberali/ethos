using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ethos.EntityFrameworkCore.Migrations
{
    public partial class SplitSchedules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "RecurringExpression",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Schedules");

            migrationBuilder.EnsureSchema(
                name: "Bookings");

            migrationBuilder.EnsureSchema(
                name: "Schedules");

            migrationBuilder.RenameTable(
                name: "Schedules",
                newName: "Schedules",
                newSchema: "Schedules");

            migrationBuilder.RenameTable(
                name: "Bookings",
                newName: "Bookings",
                newSchema: "Bookings");

            migrationBuilder.CreateTable(
                name: "Exceptions",
                schema: "Schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exceptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recurring",
                schema: "Schedules",
                columns: table => new
                {
                    ScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecurringExpression = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recurring", x => x.ScheduleId);
                });

            migrationBuilder.CreateTable(
                name: "Singles",
                schema: "Schedules",
                columns: table => new
                {
                    ScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Singles", x => x.ScheduleId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exceptions",
                schema: "Schedules");

            migrationBuilder.DropTable(
                name: "Recurring",
                schema: "Schedules");

            migrationBuilder.DropTable(
                name: "Singles",
                schema: "Schedules");

            migrationBuilder.RenameTable(
                name: "Schedules",
                schema: "Schedules",
                newName: "Schedules");

            migrationBuilder.RenameTable(
                name: "Bookings",
                schema: "Bookings",
                newName: "Bookings");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Schedules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecurringExpression",
                table: "Schedules",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Schedules",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
