using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VinhKhanhTour.CMS.Migrations
{
    /// <inheritdoc />
    public partial class AddAgencyIsolationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgencyId",
                table: "UsageHistories",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AgencyId",
                table: "Tours",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Tours",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_UsageHistories_AgencyId",
                table: "UsageHistories",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_AgencyId",
                table: "Tours",
                column: "AgencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsageHistories_AgencyId",
                table: "UsageHistories");

            migrationBuilder.DropIndex(
                name: "IX_Tours_AgencyId",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                table: "UsageHistories");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Tours");
        }
    }
}
