using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MutinyBot.Migrations
{
    public partial class Removed_MuteRoleId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MuteRoleId",
                table: "Guilds");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "MuteRoleId",
                table: "Guilds",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);
        }
    }
}
