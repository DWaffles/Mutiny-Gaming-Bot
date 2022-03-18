using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MutinyBot.Migrations
{
    public partial class ImplementedPets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PetName",
                table: "Pets",
                newName: "PetNames");

            migrationBuilder.RenameColumn(
                name: "PetImageUrl",
                table: "Pets",
                newName: "MediaUrl");

            migrationBuilder.RenameColumn(
                name: "PetId",
                table: "Pets",
                newName: "ImageId");

            migrationBuilder.AddColumn<bool>(
                name: "IsPetOwner",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPetOwner",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PetNames",
                table: "Pets",
                newName: "PetName");

            migrationBuilder.RenameColumn(
                name: "MediaUrl",
                table: "Pets",
                newName: "PetImageUrl");

            migrationBuilder.RenameColumn(
                name: "ImageId",
                table: "Pets",
                newName: "PetId");
        }
    }
}
