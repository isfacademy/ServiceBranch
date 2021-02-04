using Microsoft.EntityFrameworkCore.Migrations;

namespace ServiceBranch.Migrations
{
    public partial class AddMilitaryNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MilitaryNumber",
                table: "Substitutes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MilitaryNumber",
                table: "Substitutes");
        }
    }
}
