using Microsoft.EntityFrameworkCore.Migrations;

namespace MutinyBot.Migrations
{
    public partial class ModLogId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimesMuted",
                table: "Members",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<ulong>(
                name: "ModerationLogChannelId",
                table: "Guilds",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimesMuted",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ModerationLogChannelId",
                table: "Guilds");
        }
    }
}
