using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ethos.EntityFrameworkCore.Migrations
{
    public partial class FixDurationAsInteger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Schedules");

            migrationBuilder.AddColumn<int>(
                name: "DurationInMinutes",
                table: "Schedules",
                type: "int",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationInMinutes",
                table: "Schedules");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "Schedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
