using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyuniqueindexonWordtoincludeDictionary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Words_DictionaryId",
                table: "Words");

            migrationBuilder.CreateIndex(
                name: "IX_Words_DictionaryId_TheWord",
                table: "Words",
                columns: new[] { "DictionaryId", "TheWord" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Words_DictionaryId_TheWord",
                table: "Words");

            migrationBuilder.CreateIndex(
                name: "IX_Words_DictionaryId",
                table: "Words",
                column: "DictionaryId");
        }
    }
}
