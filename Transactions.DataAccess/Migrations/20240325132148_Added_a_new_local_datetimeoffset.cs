using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transactions.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Added_a_new_local_datetimeoffset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "TransactionDateAtLocal",
                schema: "dbo",
                table: "Transactions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionDateAtLocal",
                schema: "dbo",
                table: "Transactions");
        }
    }
}
