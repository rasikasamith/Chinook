using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chinook.Migrations
{
    /// <inheritdoc />
    public partial class ModifyTrackFavorite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TrackFavorites_TrackId",
                table: "TrackFavorites",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackFavorites_Track_TrackId",
                table: "TrackFavorites",
                column: "TrackId",
                principalTable: "Track",
                principalColumn: "TrackId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackFavorites_Track_TrackId",
                table: "TrackFavorites");

            migrationBuilder.DropIndex(
                name: "IX_TrackFavorites_TrackId",
                table: "TrackFavorites");
        }
    }
}
