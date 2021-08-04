using Microsoft.EntityFrameworkCore.Migrations;

namespace MutinyBot.Migrations
{
    public partial class ChangingGuildModelProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VerifiedPetOwner",
                table: "Members",
                newName: "LastMessageTimestampRaw");

            migrationBuilder.RenameColumn(
                name: "JoinLogEnabled",
                table: "Guilds",
                newName: "TrackMessageTimestamps");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastMessageTimestampRaw",
                table: "Members",
                newName: "VerifiedPetOwner");

            migrationBuilder.RenameColumn(
                name: "TrackMessageTimestamps",
                table: "Guilds",
                newName: "JoinLogEnabled");
        }
    }
}
