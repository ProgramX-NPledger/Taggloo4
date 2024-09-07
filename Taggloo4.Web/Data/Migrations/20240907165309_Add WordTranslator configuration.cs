using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWordTranslatorconfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TranslatorConfigurations",
                columns: new[] { "Key", "IsEnabled", "NumberOfItemsInSummary", "Priority" },
                values: new object[] { "WordTranslator", true, 6, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TranslatorConfigurations",
                keyColumn: "Key",
                keyValue: "WordTranslator");
        }
    }
}
