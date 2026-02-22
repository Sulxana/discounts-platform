using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Discounts.Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class AddCoupons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PurchasedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRedeemed = table.Column<bool>(type: "bit", nullable: false),
                    RedeemedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coupons_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Coupons_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id");
                });

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
                name: "IX_Coupons_Code",
                table: "Coupons",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_OfferId",
                table: "Coupons",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_UserId",
                table: "Coupons",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons");

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
        }
    }
}
