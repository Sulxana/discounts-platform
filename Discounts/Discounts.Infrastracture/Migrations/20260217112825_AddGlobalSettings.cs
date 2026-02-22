using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Discounts.Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class AddGlobalSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GlobalSetting",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalSetting", x => x.Key);
                });

            migrationBuilder.InsertData(
                table: "GlobalSetting",
                columns: new[] { "Key", "Description", "Type", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { "Reservation.ExpirationMinutes", "Minutes before reservation expires", "Integer", new DateTime(2026, 2, 17, 11, 28, 24, 528, DateTimeKind.Utc).AddTicks(2910), "30" },
                    { "Reservation.MaxQuantity", "Maximum quantity per reservation", "Integer", new DateTime(2026, 2, 17, 11, 28, 24, 528, DateTimeKind.Utc).AddTicks(2911), "10" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalSetting");
        }
    }
}
