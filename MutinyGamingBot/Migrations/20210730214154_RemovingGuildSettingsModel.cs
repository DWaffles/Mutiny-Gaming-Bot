using Microsoft.EntityFrameworkCore.Migrations;

namespace MutinyBot.Migrations
{
    public partial class RemovingGuildSettingsModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildSettings");

            migrationBuilder.AddColumn<ulong>(
                name: "JoinLogChannelId",
                table: "Guilds",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<bool>(
                name: "JoinLogEnabled",
                table: "Guilds",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<ulong>(
                name: "ModerationLogChannelId",
                table: "Guilds",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "MuteRoleId",
                table: "Guilds",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<bool>(
                name: "TrackMemberRoles",
                table: "Guilds",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JoinLogChannelId",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "JoinLogEnabled",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "ModerationLogChannelId",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "MuteRoleId",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "TrackMemberRoles",
                table: "Guilds");

            migrationBuilder.CreateTable(
                name: "GuildSettings",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    JoinLogChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    JoinLogEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ModerationLogChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MuteRoleId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    TrackMemberRoles = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildSettings", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_GuildSettings_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
