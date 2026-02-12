using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Discounts.Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class AddrejectMessageInOfferConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionMessage",
                table: "Offer",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionMessage",
                table: "Offer");
        }
    }
}
