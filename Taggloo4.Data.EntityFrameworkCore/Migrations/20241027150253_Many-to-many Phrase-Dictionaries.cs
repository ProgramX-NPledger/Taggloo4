using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class ManytomanyPhraseDictionaries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Phrases_Dictionaries_DictionaryId",
                table: "Phrases");

            migrationBuilder.DropIndex(
                name: "IX_Phrases_DictionaryId_ThePhrase",
                table: "Phrases");

            migrationBuilder.AlterColumn<string>(
                name: "ThePhrase",
                table: "Phrases",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "DictionaryPhrase",
                columns: table => new
                {
                    DictionariesId = table.Column<int>(type: "int", nullable: false),
                    PhrasesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DictionaryPhrase", x => new { x.DictionariesId, x.PhrasesId });
                    table.ForeignKey(
                        name: "FK_DictionaryPhrase_Dictionaries_DictionariesId",
                        column: x => x.DictionariesId,
                        principalTable: "Dictionaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DictionaryPhrase_Phrases_PhrasesId",
                        column: x => x.PhrasesId,
                        principalTable: "Phrases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryPhrase_PhrasesId",
                table: "DictionaryPhrase",
                column: "PhrasesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DictionaryPhrase");

            migrationBuilder.AlterColumn<string>(
                name: "ThePhrase",
                table: "Phrases",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Phrases_DictionaryId_ThePhrase",
                table: "Phrases",
                columns: new[] { "DictionaryId", "ThePhrase" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Phrases_Dictionaries_DictionaryId",
                table: "Phrases",
                column: "DictionaryId",
                principalTable: "Dictionaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
