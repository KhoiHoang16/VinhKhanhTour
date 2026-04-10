using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VinhKhanhTour.CMS.Migrations
{
    /// <inheritdoc />
    public partial class AddPoiAgencyIsolation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgencyId",
                table: "Pois",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Pois",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Pois_AgencyId",
                table: "Pois",
                column: "AgencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pois_AgencyId",
                table: "Pois");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                table: "Pois");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Pois");
        }
    }
}
