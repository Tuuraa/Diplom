using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WPFComponents.Migrations
{
    /// <inheritdoc />
    public partial class ForLogsFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Resonse",
                table: "Logs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "Logs",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "Logs",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Response",
                table: "Logs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "Logs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "Resonse",
                table: "Logs",
                type: "TEXT",
                nullable: true);
        }
    }
}
