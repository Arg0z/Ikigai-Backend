using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ikigai_Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAnimeAndEpisodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeasonNumber",
                table: "Episodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsOngoing",
                table: "Animes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadDate",
                table: "Animes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeasonNumber",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "IsOngoing",
                table: "Animes");

            migrationBuilder.DropColumn(
                name: "UploadDate",
                table: "Animes");
        }
    }
}
