using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingPhraseMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhraseTranslation_Dictionaries_DictionaryId",
                table: "PhraseTranslation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhraseTranslation",
                table: "PhraseTranslation");

            migrationBuilder.RenameTable(
                name: "PhraseTranslation",
                newName: "PhraseTranslations");

            migrationBuilder.RenameIndex(
                name: "IX_PhraseTranslation_DictionaryId",
                table: "PhraseTranslations",
                newName: "IX_PhraseTranslations_DictionaryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhraseTranslations",
                table: "PhraseTranslations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PhraseTranslations_Dictionaries_DictionaryId",
                table: "PhraseTranslations",
                column: "DictionaryId",
                principalTable: "Dictionaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhraseTranslations_Dictionaries_DictionaryId",
                table: "PhraseTranslations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhraseTranslations",
                table: "PhraseTranslations");

            migrationBuilder.RenameTable(
                name: "PhraseTranslations",
                newName: "PhraseTranslation");

            migrationBuilder.RenameIndex(
                name: "IX_PhraseTranslations_DictionaryId",
                table: "PhraseTranslation",
                newName: "IX_PhraseTranslation_DictionaryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhraseTranslation",
                table: "PhraseTranslation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PhraseTranslation_Dictionaries_DictionaryId",
                table: "PhraseTranslation",
                column: "DictionaryId",
                principalTable: "Dictionaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
