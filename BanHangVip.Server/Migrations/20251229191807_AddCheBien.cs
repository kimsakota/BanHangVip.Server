using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanHangVip.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddCheBien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Preparation",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Preparation",
                table: "Products");
        }
    }
}
