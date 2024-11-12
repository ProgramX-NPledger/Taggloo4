using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Correctdirectionofcascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DictionaryWord_Dictionaries_DictionariesId",
                table: "DictionaryWord");

            migrationBuilder.DropForeignKey(
                name: "FK_DictionaryWord_Words_WordsId",
                table: "DictionaryWord");

            migrationBuilder.AddForeignKey(
                name: "FK_DictionaryWord_Dictionaries_DictionariesId",
                table: "DictionaryWord",
                column: "DictionariesId",
                principalTable: "Dictionaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DictionaryWord_Words_WordsId",
                table: "DictionaryWord",
                column: "WordsId",
                principalTable: "Words",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DictionaryWord_Dictionaries_DictionariesId",
                table: "DictionaryWord");

            migrationBuilder.DropForeignKey(
                name: "FK_DictionaryWord_Words_WordsId",
                table: "DictionaryWord");

            migrationBuilder.AddForeignKey(
                name: "FK_DictionaryWord_Dictionaries_DictionariesId",
                table: "DictionaryWord",
                column: "DictionariesId",
                principalTable: "Dictionaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DictionaryWord_Words_WordsId",
                table: "DictionaryWord",
                column: "WordsId",
                principalTable: "Words",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
