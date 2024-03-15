using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseProject_CommandLineDBManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class updateprimarykeyleagueteam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LeagueTeams",
                table: "LeagueTeams");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "LeagueTeams",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeagueTeams",
                table: "LeagueTeams",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LeagueTeams_LeagueId",
                table: "LeagueTeams",
                column: "LeagueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LeagueTeams",
                table: "LeagueTeams");

            migrationBuilder.DropIndex(
                name: "IX_LeagueTeams_LeagueId",
                table: "LeagueTeams");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LeagueTeams");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeagueTeams",
                table: "LeagueTeams",
                columns: new[] { "LeagueId", "TeamId" });
        }
    }
}
