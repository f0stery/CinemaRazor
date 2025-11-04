using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaRazor.Migrations
{
    /// <inheritdoc />
    public partial class FixSeatLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Удаляем связь Seat -> Session
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Sessions_SessionId",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Seats_SessionId",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "IsOccupied",
                table: "Seats");

            // Добавляем уникальный индекс для комбинации SessionId + SeatId в Tickets
            migrationBuilder.CreateIndex(
                name: "IX_Tickets_SessionId_SeatId",
                table: "Tickets",
                columns: new[] { "SessionId", "SeatId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tickets_SessionId_SeatId",
                table: "Tickets");

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "Seats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsOccupied",
                table: "Seats",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Seats_SessionId",
                table: "Seats",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Sessions_SessionId",
                table: "Seats",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
