using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogIt.Data.Migrations
{
    public partial class AddSocialMediaICons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Instagram",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Twitter",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Youtube",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Facebook",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Instagram",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Twitter",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Youtube",
                table: "UserProfiles");
        }
    }
}
