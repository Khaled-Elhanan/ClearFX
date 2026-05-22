using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearFX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExchangeRateProviderFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "SetBy",
                table: "ExchangeRates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "ExternalReferenceId",
                table: "ExchangeRates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "FetchedAt",
                table: "ExchangeRates",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderName",
                table: "ExchangeRates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalReferenceId",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "FetchedAt",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "ProviderName",
                table: "ExchangeRates");

            migrationBuilder.AlterColumn<Guid>(
                name: "SetBy",
                table: "ExchangeRates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
