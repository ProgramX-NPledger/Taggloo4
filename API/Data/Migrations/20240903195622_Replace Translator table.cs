using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceTranslatortable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Translators");

            migrationBuilder.CreateTable(
                name: "TranslatorConfigurations",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslatorConfigurations", x => x.Key);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhraseTranslations_ToPhraseId",
                table: "PhraseTranslations",
                column: "ToPhraseId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhraseTranslations_Phrases_ToPhraseId",
                table: "PhraseTranslations",
                column: "ToPhraseId",
                principalTable: "Phrases",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhraseTranslations_Phrases_ToPhraseId",
                table: "PhraseTranslations");

            migrationBuilder.DropTable(
                name: "TranslatorConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_PhraseTranslations_ToPhraseId",
                table: "PhraseTranslations");

            migrationBuilder.CreateTable(
                name: "Translators",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translators", x => x.Key);
                });
        }
    }
}
