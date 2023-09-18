using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chinook.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackFavorite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrackFavorites",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    TrackId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackFavorites", x => new { x.UserId, x.TrackId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackFavorites");
        }
    }
}
