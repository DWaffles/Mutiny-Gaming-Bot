using Microsoft.EntityFrameworkCore.Migrations;

namespace MutinyBot.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    GuildName = table.Column<string>(type: "TEXT", nullable: true),
                    JoinLogEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    JoinLogChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MuteRoleId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MemberId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    TimesJoined = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentMember = table.Column<bool>(type: "INTEGER", nullable: false),
                    RoleDictionary = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Banned = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEntity", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_GuildId",
                table: "Guilds",
                column: "GuildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_MemberId_GuildId",
                table: "Members",
                columns: new[] { "MemberId", "GuildId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserEntity_UserId",
                table: "UserEntity",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "UserEntity");
        }
    }
}
