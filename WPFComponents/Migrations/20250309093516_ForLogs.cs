using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WPFComponents.Migrations
{
    /// <inheritdoc />
    public partial class ForLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Resonse",
                table: "Logs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "Logs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Resonse",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "Result",
                table: "Logs");
        }
    }
}
