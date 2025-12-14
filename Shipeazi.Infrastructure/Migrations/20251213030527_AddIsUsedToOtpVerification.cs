using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shipeazi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsUsedToOtpVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "OtpVerifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UsedAt",
                table: "OtpVerifications",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "OtpVerifications");

            migrationBuilder.DropColumn(
                name: "UsedAt",
                table: "OtpVerifications");
        }
    }
}
