using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class ReindexingJobtracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReindexingJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartedOn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfLanguagesProcessed = table.Column<int>(type: "int", nullable: true),
                    NumberOfDictionariesProcessed = table.Column<int>(type: "int", nullable: true),
                    NumberOfPhrasesProcessed = table.Column<int>(type: "int", nullable: true),
                    NumberOfWordProcessed = table.Column<int>(type: "int", nullable: true),
                    NumberOfWordsInPhrasesCreated = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReindexingJobs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReindexingJobs");
        }
    }
}
