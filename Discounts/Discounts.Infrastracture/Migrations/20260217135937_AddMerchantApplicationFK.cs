using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Discounts.Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class AddMerchantApplicationFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OfferId1",
                table: "Reservations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Merchant.EditWindowHours",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 17, 13, 59, 37, 314, DateTimeKind.Utc).AddTicks(481));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.ExpirationMinutes",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 17, 13, 59, 37, 314, DateTimeKind.Utc).AddTicks(477));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.MaxQuantity",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 17, 13, 59, 37, 314, DateTimeKind.Utc).AddTicks(480));

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_OfferId1",
                table: "Reservations",
                column: "OfferId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Offer_OfferId1",
                table: "Reservations",
                column: "OfferId1",
                principalTable: "Offer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Offer_OfferId1",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_OfferId1",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "OfferId1",
                table: "Reservations");

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Merchant.EditWindowHours",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 17, 13, 24, 44, 946, DateTimeKind.Utc).AddTicks(9234));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.ExpirationMinutes",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 17, 13, 24, 44, 946, DateTimeKind.Utc).AddTicks(9229));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.MaxQuantity",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 17, 13, 24, 44, 946, DateTimeKind.Utc).AddTicks(9233));
        }
    }
}
