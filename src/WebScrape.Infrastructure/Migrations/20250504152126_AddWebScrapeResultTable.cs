using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebScrape.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWebScrapeResultTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "WebScrapeResults",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "WebScrapeResults");
        }
    }
}
