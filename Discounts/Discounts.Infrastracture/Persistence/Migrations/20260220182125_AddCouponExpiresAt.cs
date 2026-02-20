using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Discounts.Infrastracture.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCouponExpiresAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "Coupons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Merchant.EditWindowHours",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 18, 21, 24, 695, DateTimeKind.Utc).AddTicks(3175));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.ExpirationMinutes",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 18, 21, 24, 695, DateTimeKind.Utc).AddTicks(3171));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.MaxQuantity",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 18, 21, 24, 695, DateTimeKind.Utc).AddTicks(3174));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "Coupons");

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
    }
}
