using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shipeazi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OtpVerifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CountryCode = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    OtpCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AttemptsCount = table.Column<int>(type: "integer", nullable: false),
                    Purpose = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpVerifications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_ExpiresAt",
                table: "OtpVerifications",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_IsVerified",
                table: "OtpVerifications",
                column: "IsVerified");

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_PhoneNumber_CountryCode",
                table: "OtpVerifications",
                columns: new[] { "PhoneNumber", "CountryCode" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OtpVerifications");
        }
    }
}
