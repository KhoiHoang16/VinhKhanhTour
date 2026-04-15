using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VinhKhanhTour.CMS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTouristForTraditionalLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM \"Tourists\";");

            migrationBuilder.DropIndex(
                name: "IX_Tourists_Email",
                table: "Tourists");

            migrationBuilder.DropIndex(
                name: "IX_Tourists_SocialId",
                table: "Tourists");

            migrationBuilder.DropColumn(
                name: "AuthProvider",
                table: "Tourists");

            migrationBuilder.DropColumn(
                name: "SocialId",
                table: "Tourists");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Tourists",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tourists_Email",
                table: "Tourists",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tourists_Email",
                table: "Tourists");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Tourists");

            migrationBuilder.AddColumn<string>(
                name: "AuthProvider",
                table: "Tourists",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SocialId",
                table: "Tourists",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tourists_Email",
                table: "Tourists",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Tourists_SocialId",
                table: "Tourists",
                column: "SocialId",
                unique: true);
        }
    }
}
