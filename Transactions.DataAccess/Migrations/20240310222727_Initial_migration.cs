using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transactions.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Transactions",
                schema: "dbo",
                columns: table => new
                {
                    TransactionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    TransactionDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ClientLocation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions",
                schema: "dbo");
        }
    }
}
