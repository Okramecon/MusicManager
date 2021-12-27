using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicManager.Migrations
{
    public partial class ConfigureTrackOnDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Albums_AlbumId",
                table: "Tracks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Authors_AuthorId",
                table: "Tracks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Users_OwnerId",
                table: "Tracks");

            migrationBuilder.DropIndex(
                name: "IX_Tracks_OwnerId",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Tracks");

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Albums_AlbumId",
                table: "Tracks",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Authors_AuthorId",
                table: "Tracks",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Albums_AlbumId",
                table: "Tracks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Authors_AuthorId",
                table: "Tracks");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Tracks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_OwnerId",
                table: "Tracks",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Albums_AlbumId",
                table: "Tracks",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Authors_AuthorId",
                table: "Tracks",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Users_OwnerId",
                table: "Tracks",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
