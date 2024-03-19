using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transactions.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Rework_TimeZone_toObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TimeZone",
                schema: "dbo",
                table: "Transactions",
                type: "nvarchar(MAX)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TimeZone",
                schema: "dbo",
                table: "Transactions",
                type: "varchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(MAX)");
        }
    }
}
