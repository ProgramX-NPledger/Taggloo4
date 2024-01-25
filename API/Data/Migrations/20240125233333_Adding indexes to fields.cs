using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class Addingindexestofields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordTranslation_Dictionaries_DictionaryId",
                table: "WordTranslation");

            migrationBuilder.DropForeignKey(
                name: "FK_WordTranslation_Words_FromWordId",
                table: "WordTranslation");

            migrationBuilder.DropForeignKey(
                name: "FK_WordTranslation_Words_ToWordId",
                table: "WordTranslation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WordTranslation",
                table: "WordTranslation");

            migrationBuilder.RenameTable(
                name: "WordTranslation",
                newName: "WordTranslations");

            migrationBuilder.RenameIndex(
                name: "IX_WordTranslation_ToWordId",
                table: "WordTranslations",
                newName: "IX_WordTranslations_ToWordId");

            migrationBuilder.RenameIndex(
                name: "IX_WordTranslation_FromWordId",
                table: "WordTranslations",
                newName: "IX_WordTranslations_FromWordId");

            migrationBuilder.RenameIndex(
                name: "IX_WordTranslation_DictionaryId",
                table: "WordTranslations",
                newName: "IX_WordTranslations_DictionaryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WordTranslations",
                table: "WordTranslations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WordTranslations_Dictionaries_DictionaryId",
                table: "WordTranslations",
                column: "DictionaryId",
                principalTable: "Dictionaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WordTranslations_Words_FromWordId",
                table: "WordTranslations",
                column: "FromWordId",
                principalTable: "Words",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WordTranslations_Words_ToWordId",
                table: "WordTranslations",
                column: "ToWordId",
                principalTable: "Words",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordTranslations_Dictionaries_DictionaryId",
                table: "WordTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_WordTranslations_Words_FromWordId",
                table: "WordTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_WordTranslations_Words_ToWordId",
                table: "WordTranslations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WordTranslations",
                table: "WordTranslations");

            migrationBuilder.RenameTable(
                name: "WordTranslations",
                newName: "WordTranslation");

            migrationBuilder.RenameIndex(
                name: "IX_WordTranslations_ToWordId",
                table: "WordTranslation",
                newName: "IX_WordTranslation_ToWordId");

            migrationBuilder.RenameIndex(
                name: "IX_WordTranslations_FromWordId",
                table: "WordTranslation",
                newName: "IX_WordTranslation_FromWordId");

            migrationBuilder.RenameIndex(
                name: "IX_WordTranslations_DictionaryId",
                table: "WordTranslation",
                newName: "IX_WordTranslation_DictionaryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WordTranslation",
                table: "WordTranslation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WordTranslation_Dictionaries_DictionaryId",
                table: "WordTranslation",
                column: "DictionaryId",
                principalTable: "Dictionaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WordTranslation_Words_FromWordId",
                table: "WordTranslation",
                column: "FromWordId",
                principalTable: "Words",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WordTranslation_Words_ToWordId",
                table: "WordTranslation",
                column: "ToWordId",
                principalTable: "Words",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
