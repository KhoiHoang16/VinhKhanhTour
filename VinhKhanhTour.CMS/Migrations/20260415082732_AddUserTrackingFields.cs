using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VinhKhanhTour.CMS.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTrackingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Tourists",
                newName: "IsLocked");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActiveAt",
                table: "Tourists",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastActiveDevice",
                table: "Tourists",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastActiveIp",
                table: "Tourists",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "AppUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActiveAt",
                table: "AppUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastActiveDevice",
                table: "AppUsers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastActiveIp",
                table: "AppUsers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastActiveAt",
                table: "Tourists");

            migrationBuilder.DropColumn(
                name: "LastActiveDevice",
                table: "Tourists");

            migrationBuilder.DropColumn(
                name: "LastActiveIp",
                table: "Tourists");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "LastActiveAt",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "LastActiveDevice",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "LastActiveIp",
                table: "AppUsers");

            migrationBuilder.RenameColumn(
                name: "IsLocked",
                table: "Tourists",
                newName: "IsActive");
        }
    }
}
