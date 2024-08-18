using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedToWordtoWord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WordTranslations_ToWordId",
                table: "WordTranslations",
                column: "ToWordId");

            migrationBuilder.AddForeignKey(
                name: "FK_WordTranslations_Words_ToWordId",
                table: "WordTranslations",
                column: "ToWordId",
                principalTable: "Words",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordTranslations_Words_ToWordId",
                table: "WordTranslations");

            migrationBuilder.DropIndex(
                name: "IX_WordTranslations_ToWordId",
                table: "WordTranslations");
        }
    }
}
