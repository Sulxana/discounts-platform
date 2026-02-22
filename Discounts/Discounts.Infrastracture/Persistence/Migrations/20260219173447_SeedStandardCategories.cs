using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Discounts.Infrastracture.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedStandardCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Restaurant" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Fitness" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Tourism" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Entertainment" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Services" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "Education" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), "Other" }
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Merchant.EditWindowHours",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 19, 13, 34, 16, 101, DateTimeKind.Utc).AddTicks(9209));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.ExpirationMinutes",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 19, 13, 34, 16, 101, DateTimeKind.Utc).AddTicks(9204));

            migrationBuilder.UpdateData(
                table: "GlobalSetting",
                keyColumn: "Key",
                keyValue: "Reservation.MaxQuantity",
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 19, 13, 34, 16, 101, DateTimeKind.Utc).AddTicks(9208));
        }
    }
}
