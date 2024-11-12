using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class ManytomanyWordDictionaries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Words_Dictionaries_DictionaryId",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_DictionaryId_TheWord",
                table: "Words");

            migrationBuilder.CreateTable(
                name: "DictionaryWord",
                columns: table => new
                {
                    DictionariesId = table.Column<int>(type: "int", nullable: false),
                    WordsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DictionaryWord", x => new { x.DictionariesId, x.WordsId });
                    table.ForeignKey(
                        name: "FK_DictionaryWord_Dictionaries_DictionariesId",
                        column: x => x.DictionariesId,
                        principalTable: "Dictionaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DictionaryWord_Words_WordsId",
                        column: x => x.WordsId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryWord_WordsId",
                table: "DictionaryWord",
                column: "WordsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DictionaryWord");

            migrationBuilder.CreateIndex(
                name: "IX_Words_DictionaryId_TheWord",
                table: "Words",
                columns: new[] { "DictionaryId", "TheWord" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Words_Dictionaries_DictionaryId",
                table: "Words",
                column: "DictionaryId",
                principalTable: "Dictionaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
