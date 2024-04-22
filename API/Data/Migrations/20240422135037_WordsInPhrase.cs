using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class WordsInPhrase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WordsInPhrase");

            migrationBuilder.CreateTable(
                name: "WordsInPhrases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InPhraseId = table.Column<int>(type: "int", nullable: false),
                    WordId = table.Column<int>(type: "int", nullable: false),
                    Ordinal = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordsInPhrases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordsInPhrases_Phrases_InPhraseId",
                        column: x => x.InPhraseId,
                        principalTable: "Phrases",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WordsInPhrases_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WordTranslations_FromWordId",
                table: "WordTranslations",
                column: "FromWordId");

            migrationBuilder.CreateIndex(
                name: "IX_PhraseTranslations_FromPhraseId",
                table: "PhraseTranslations",
                column: "FromPhraseId");

            migrationBuilder.CreateIndex(
                name: "IX_WordsInPhrases_InPhraseId",
                table: "WordsInPhrases",
                column: "InPhraseId");

            migrationBuilder.CreateIndex(
                name: "IX_WordsInPhrases_WordId",
                table: "WordsInPhrases",
                column: "WordId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhraseTranslations_Phrases_FromPhraseId",
                table: "PhraseTranslations",
                column: "FromPhraseId",
                principalTable: "Phrases",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WordTranslations_Words_FromWordId",
                table: "WordTranslations",
                column: "FromWordId",
                principalTable: "Words",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhraseTranslations_Phrases_FromPhraseId",
                table: "PhraseTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_WordTranslations_Words_FromWordId",
                table: "WordTranslations");

            migrationBuilder.DropTable(
                name: "PhraseWord");

            migrationBuilder.DropTable(
                name: "WordsInPhrases");

            migrationBuilder.DropIndex(
                name: "IX_WordTranslations_FromWordId",
                table: "WordTranslations");

            migrationBuilder.DropIndex(
                name: "IX_PhraseTranslations_FromPhraseId",
                table: "PhraseTranslations");

            migrationBuilder.CreateTable(
                name: "WordsInPhrase",
                columns: table => new
                {
                    PhrasesId = table.Column<int>(type: "int", nullable: false),
                    WordsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordsInPhrase", x => new { x.PhrasesId, x.WordsId });
                    table.ForeignKey(
                        name: "FK_WordsInPhrase_Phrases_PhrasesId",
                        column: x => x.PhrasesId,
                        principalTable: "Phrases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordsInPhrase_Words_WordsId",
                        column: x => x.WordsId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WordsInPhrase_WordsId",
                table: "WordsInPhrase",
                column: "WordsId");
        }
    }
}
