using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class SeedingContentTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ContentTypes",
                columns: new[] { "Id", "ContentTypeKey", "Controller", "NamePlural", "NameSingular" },
                values: new object[,]
                {
                    { 1, "Word", "words", "Words", "Word" },
                    { 2, "WordTranslation", "wordTranslations", "Word Translations", "Word Translation" },
                    { 3, "PhraseTranslation", "phraseTranslations", "Phrase Translations", "Phrase Translation" },
                    { 4, "Phrase", "phrases", "Phrases", "Phrase" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ContentTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ContentTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ContentTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ContentTypes",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
