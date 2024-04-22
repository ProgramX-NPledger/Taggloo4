using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddfieldstoReindexingJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfWordsCreated",
                table: "ReindexingJobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfWordsInPhrasesRemoved",
                table: "ReindexingJobs",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfWordsCreated",
                table: "ReindexingJobs");

            migrationBuilder.DropColumn(
                name: "NumberOfWordsInPhrasesRemoved",
                table: "ReindexingJobs");
        }
    }
}
