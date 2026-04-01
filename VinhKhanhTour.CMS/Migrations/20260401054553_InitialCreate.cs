using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VinhKhanhTour.CMS.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pois",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: false),
                    NameEs = table.Column<string>(type: "text", nullable: false),
                    NameFr = table.Column<string>(type: "text", nullable: false),
                    NameDe = table.Column<string>(type: "text", nullable: false),
                    NameZh = table.Column<string>(type: "text", nullable: false),
                    NameJa = table.Column<string>(type: "text", nullable: false),
                    NameKo = table.Column<string>(type: "text", nullable: false),
                    NameRu = table.Column<string>(type: "text", nullable: false),
                    NameIt = table.Column<string>(type: "text", nullable: false),
                    NamePt = table.Column<string>(type: "text", nullable: false),
                    PrimaryCategory = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DescriptionEn = table.Column<string>(type: "text", nullable: false),
                    DescriptionEs = table.Column<string>(type: "text", nullable: false),
                    DescriptionFr = table.Column<string>(type: "text", nullable: false),
                    DescriptionDe = table.Column<string>(type: "text", nullable: false),
                    DescriptionZh = table.Column<string>(type: "text", nullable: false),
                    DescriptionJa = table.Column<string>(type: "text", nullable: false),
                    DescriptionKo = table.Column<string>(type: "text", nullable: false),
                    DescriptionRu = table.Column<string>(type: "text", nullable: false),
                    DescriptionIt = table.Column<string>(type: "text", nullable: false),
                    DescriptionPt = table.Column<string>(type: "text", nullable: false),
                    TtsScript = table.Column<string>(type: "text", nullable: false),
                    TtsScriptEn = table.Column<string>(type: "text", nullable: false),
                    TtsScriptEs = table.Column<string>(type: "text", nullable: false),
                    TtsScriptFr = table.Column<string>(type: "text", nullable: false),
                    TtsScriptDe = table.Column<string>(type: "text", nullable: false),
                    TtsScriptZh = table.Column<string>(type: "text", nullable: false),
                    TtsScriptJa = table.Column<string>(type: "text", nullable: false),
                    TtsScriptKo = table.Column<string>(type: "text", nullable: false),
                    TtsScriptRu = table.Column<string>(type: "text", nullable: false),
                    TtsScriptIt = table.Column<string>(type: "text", nullable: false),
                    TtsScriptPt = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    AudioUrlVi = table.Column<string>(type: "text", nullable: false),
                    AudioUrlEn = table.Column<string>(type: "text", nullable: false),
                    Radius = table.Column<double>(type: "double precision", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pois", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TourStops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TourId = table.Column<int>(type: "integer", nullable: false),
                    PoiId = table.Column<int>(type: "integer", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourStops", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsageHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceId = table.Column<string>(type: "text", nullable: false),
                    PoiId = table.Column<int>(type: "integer", nullable: false),
                    ListenDurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    IsQrTriggered = table.Column<bool>(type: "boolean", nullable: false),
                    UserLatitude = table.Column<double>(type: "double precision", nullable: false),
                    UserLongitude = table.Column<double>(type: "double precision", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsageHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceId = table.Column<string>(type: "text", nullable: false),
                    RouteDataJson = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoutes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pois");

            migrationBuilder.DropTable(
                name: "Tours");

            migrationBuilder.DropTable(
                name: "TourStops");

            migrationBuilder.DropTable(
                name: "UsageHistories");

            migrationBuilder.DropTable(
                name: "UserRoutes");
        }
    }
}
