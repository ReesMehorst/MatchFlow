using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TeamMemberCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Teams_Tag",
                table: "Teams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_TeamId_UserId_LeftAt",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TeamMembers");

            migrationBuilder.AlterColumn<string>(
                name: "Tag",
                table: "Teams",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers",
                columns: new[] { "TeamId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers");

            migrationBuilder.AlterColumn<string>(
                name: "Tag",
                table: "Teams",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "TeamMembers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Tag",
                table: "Teams",
                column: "Tag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_TeamId_UserId_LeftAt",
                table: "TeamMembers",
                columns: new[] { "TeamId", "UserId", "LeftAt" });
        }
    }
}
