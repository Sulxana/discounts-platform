using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Discounts.Infrastracture.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Merchant.EditWindowHours",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 16, 51, 2, 785, DateTimeKind.Utc).AddTicks(9316));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.ExpirationMinutes",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 16, 51, 2, 785, DateTimeKind.Utc).AddTicks(9311));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.MaxQuantity",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 16, 51, 2, 785, DateTimeKind.Utc).AddTicks(9316));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Merchant.EditWindowHours",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 19, 17, 34, 46, 985, DateTimeKind.Utc).AddTicks(157));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.ExpirationMinutes",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 19, 17, 34, 46, 985, DateTimeKind.Utc).AddTicks(152));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.MaxQuantity",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 19, 17, 34, 46, 985, DateTimeKind.Utc).AddTicks(156));
        }
    }
}
