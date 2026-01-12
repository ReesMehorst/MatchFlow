using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.AddColumn<Guid>(
                name: "TeamAId",
                table: "Fixtures",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamAScore",
                table: "Fixtures",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamBId",
                table: "Fixtures",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamBScore",
                table: "Fixtures",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TournamentId",
                table: "Fixtures",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_TournamentId",
                table: "Fixtures",
                column: "TournamentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fixtures_Tournaments_TournamentId",
                table: "Fixtures",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fixtures_Tournaments_TournamentId",
                table: "Fixtures");

            migrationBuilder.DropIndex(
                name: "IX_Fixtures_TournamentId",
                table: "Fixtures");

            migrationBuilder.DropColumn(
                name: "TeamAId",
                table: "Fixtures");

            migrationBuilder.DropColumn(
                name: "TeamAScore",
                table: "Fixtures");

            migrationBuilder.DropColumn(
                name: "TeamBId",
                table: "Fixtures");

            migrationBuilder.DropColumn(
                name: "TeamBScore",
                table: "Fixtures");

            migrationBuilder.DropColumn(
                name: "TournamentId",
                table: "Fixtures");

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamAId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TeamAScore = table.Column<int>(type: "int", nullable: true),
                    TeamBId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TeamBScore = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_TournamentId",
                table: "Matches",
                column: "TournamentId");
        }
    }
}
