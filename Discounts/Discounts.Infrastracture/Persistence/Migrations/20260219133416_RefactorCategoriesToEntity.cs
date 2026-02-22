using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Discounts.Infrastracture.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorCategoriesToEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Offer_Status_Category_EndDate",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Offer");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Offer",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // SEED DATA: Insert default category
            var defaultCategoryId = Guid.NewGuid();
            migrationBuilder.Sql($"INSERT INTO Categories (Id, Name) VALUES ('{defaultCategoryId}', 'General')");

            // DATA MIGRATION: Update existing offers to point to default category
            migrationBuilder.Sql($"UPDATE Offer SET CategoryId = '{defaultCategoryId}'");

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

            migrationBuilder.CreateIndex(
                name: "IX_Offer_CategoryId",
                table: "Offer",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_Status_CategoryId_EndDate",
                table: "Offer",
                columns: new[] { "Status", "CategoryId", "EndDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Categories_CategoryId",
                table: "Offer",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Categories_CategoryId",
                table: "Offer");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Offer_CategoryId",
                table: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_Offer_Status_CategoryId_EndDate",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Offer");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Offer",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.CreateIndex(
                name: "IX_Offer_Status_Category_EndDate",
                table: "Offer",
                columns: new[] { "Status", "Category", "EndDate" });
        }
    }
}
