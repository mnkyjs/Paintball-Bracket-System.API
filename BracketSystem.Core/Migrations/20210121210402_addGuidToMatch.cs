using Microsoft.EntityFrameworkCore.Migrations;

namespace BracketSystem.Core.Migrations
{
    public partial class addGuidToMatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "Matches",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Matches");
        }
    }
}
