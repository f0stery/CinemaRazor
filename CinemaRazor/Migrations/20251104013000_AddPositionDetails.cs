using Microsoft.EntityFrameworkCore.Migrations;

namespace CinemaRazor.Migrations
{
    public partial class AddPositionDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Requirements",
                table: "Positions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "Responsibilities",
                table: "Positions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: string.Empty);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Requirements",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "Responsibilities",
                table: "Positions");
        }
    }
}
