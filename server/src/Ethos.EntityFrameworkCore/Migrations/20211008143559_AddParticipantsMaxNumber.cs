using Microsoft.EntityFrameworkCore.Migrations;

namespace Ethos.EntityFrameworkCore.Migrations
{
    public partial class AddParticipantsMaxNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParticipantsMaxNumber",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParticipantsMaxNumber",
                table: "Schedules");
        }
    }
}
