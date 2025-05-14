using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebScrape.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OneToMasny_SearchJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchResults_SearchJobs_SearchJobId",
                table: "SearchResults");

            migrationBuilder.AlterColumn<int>(
                name: "SearchJobId",
                table: "SearchResults",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SearchResults_SearchJobs_SearchJobId",
                table: "SearchResults",
                column: "SearchJobId",
                principalTable: "SearchJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchResults_SearchJobs_SearchJobId",
                table: "SearchResults");

            migrationBuilder.AlterColumn<int>(
                name: "SearchJobId",
                table: "SearchResults",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchResults_SearchJobs_SearchJobId",
                table: "SearchResults",
                column: "SearchJobId",
                principalTable: "SearchJobs",
                principalColumn: "Id");
        }
    }
}
