using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Discounts.Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Offer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: true),
                    OriginalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountedPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCoupons = table.Column<int>(type: "int", nullable: false),
                    RemainingCoupons = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offer", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offer_EndDate",
                table: "Offer",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_Status",
                table: "Offer",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_Status_Category_EndDate",
                table: "Offer",
                columns: new[] { "Status", "Category", "EndDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Offer");
        }
    }
}
