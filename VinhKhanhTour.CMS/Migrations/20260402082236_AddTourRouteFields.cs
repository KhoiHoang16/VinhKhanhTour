using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VinhKhanhTour.CMS.Migrations
{
    /// <inheritdoc />
    public partial class AddTourRouteFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Tours",
                newName: "TourName");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TourStops",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "TourStops",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "TourStops",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "PoiName",
                table: "TourStops",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EndPoint",
                table: "Tours",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartPoint",
                table: "Tours",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "TotalDistance",
                table: "Tours",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_TourStops_TourId",
                table: "TourStops",
                column: "TourId");

            migrationBuilder.AddForeignKey(
                name: "FK_TourStops_Tours_TourId",
                table: "TourStops",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TourStops_Tours_TourId",
                table: "TourStops");

            migrationBuilder.DropIndex(
                name: "IX_TourStops_TourId",
                table: "TourStops");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "TourStops");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "TourStops");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "TourStops");

            migrationBuilder.DropColumn(
                name: "PoiName",
                table: "TourStops");

            migrationBuilder.DropColumn(
                name: "EndPoint",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "StartPoint",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "TotalDistance",
                table: "Tours");

            migrationBuilder.RenameColumn(
                name: "TourName",
                table: "Tours",
                newName: "Name");
        }
    }
}
