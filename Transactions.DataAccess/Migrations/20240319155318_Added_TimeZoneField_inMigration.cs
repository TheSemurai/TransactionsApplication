using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transactions.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Added_TimeZoneField_inMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "TransactionDate",
                schema: "dbo",
                table: "Transactions",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                schema: "dbo",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZone",
                schema: "dbo",
                table: "Transactions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TransactionDate",
                schema: "dbo",
                table: "Transactions",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");
        }
    }
}
