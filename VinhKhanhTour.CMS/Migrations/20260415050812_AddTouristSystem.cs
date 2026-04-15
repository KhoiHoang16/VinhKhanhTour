using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VinhKhanhTour.CMS.Migrations
{
    /// <inheritdoc />
    public partial class AddTouristSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TouristId",
                table: "DevicePurchases",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tourists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SocialId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AvatarUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AuthProvider = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tourists", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DevicePurchases_TouristId",
                table: "DevicePurchases",
                column: "TouristId");

            migrationBuilder.CreateIndex(
                name: "IX_Tourists_Email",
                table: "Tourists",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Tourists_SocialId",
                table: "Tourists",
                column: "SocialId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DevicePurchases_Tourists_TouristId",
                table: "DevicePurchases",
                column: "TouristId",
                principalTable: "Tourists",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DevicePurchases_Tourists_TouristId",
                table: "DevicePurchases");

            migrationBuilder.DropTable(
                name: "Tourists");

            migrationBuilder.DropIndex(
                name: "IX_DevicePurchases_TouristId",
                table: "DevicePurchases");

            migrationBuilder.DropColumn(
                name: "TouristId",
                table: "DevicePurchases");
        }
    }
}
