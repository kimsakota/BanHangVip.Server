using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanHangVip.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddPinCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PinCode",
                table: "Customers",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PinCode",
                table: "Customers");
        }
    }
}
