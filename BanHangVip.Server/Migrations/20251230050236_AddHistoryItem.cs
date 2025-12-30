using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanHangVip.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoryItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "HistoryItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "HistoryItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "HistoryItems");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "HistoryItems");
        }
    }
}
