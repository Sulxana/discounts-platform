using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Discounts.Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtToOffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Offer",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.ExpirationMinutes",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 17, 11, 43, 21, 62, DateTimeKind.Utc).AddTicks(1943));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.MaxQuantity",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 17, 11, 43, 21, 62, DateTimeKind.Utc).AddTicks(1948));

            migrationBuilder.InsertData(
                table: "GlobalSetting",
                columns: new[] { "Key", "Description", "Type", "UpdatedAt", "Value" },
                values: new object[] { "Merchant.EditWindowHours", "Hours after creation when merchant can edit offer", "Integer", new DateTime(2026, 2, 17, 11, 43, 21, 62, DateTimeKind.Utc).AddTicks(1949), "24" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Merchant.EditWindowHours");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Offer");

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.ExpirationMinutes",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 17, 11, 28, 24, 528, DateTimeKind.Utc).AddTicks(2910));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.MaxQuantity",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 17, 11, 28, 24, 528, DateTimeKind.Utc).AddTicks(2911));
        }
    }
}
