using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Discounts.Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class AddMerchantIdToOffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<Guid>(
                name: "MerchantId",
                table: "Offer",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Merchant.EditWindowHours",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 18, 19, 42, 2, 22, DateTimeKind.Utc).AddTicks(1800));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.ExpirationMinutes",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 18, 19, 42, 2, 22, DateTimeKind.Utc).AddTicks(1796));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.MaxQuantity",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 18, 19, 42, 2, 22, DateTimeKind.Utc).AddTicks(1799));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MerchantId",
                table: "Offer");

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
                value: new DateTime(2026, 2, 18, 18, 53, 41, 894, DateTimeKind.Utc).AddTicks(4231));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.ExpirationMinutes",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 18, 18, 53, 41, 894, DateTimeKind.Utc).AddTicks(4223));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.MaxQuantity",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 18, 18, 53, 41, 894, DateTimeKind.Utc).AddTicks(4230));

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
    }
}
