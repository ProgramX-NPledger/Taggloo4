using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddContentTypeManagers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ContentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ContentTypeManagerDotNetAssemblyName", "ContentTypeManagerDotNetTypeName" },
                values: new object[] { "Taggloo4.Translation", "Taggloo4.Translation.ContentTypes.WordsContentTypeManager" });

            migrationBuilder.UpdateData(
                table: "ContentTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ContentTypeManagerDotNetAssemblyName", "ContentTypeManagerDotNetTypeName" },
                values: new object[] { "Taggloo4.Translation", "Taggloo4.Translation.ContentTypes.WordTranslationsContentTypeManager" });

            migrationBuilder.UpdateData(
                table: "ContentTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ContentTypeManagerDotNetAssemblyName", "ContentTypeManagerDotNetTypeName" },
                values: new object[] { "Taggloo4.Translation", "Taggloo4.Translation.ContentTypes.PhraseTranslationsContentTypeManager" });

            migrationBuilder.UpdateData(
                table: "ContentTypes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ContentTypeManagerDotNetAssemblyName", "ContentTypeManagerDotNetTypeName" },
                values: new object[] { "Taggloo4.Translation", "Taggloo4.Translation.ContentTypes.PhrasesContentTypeManager" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ContentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ContentTypeManagerDotNetAssemblyName", "ContentTypeManagerDotNetTypeName" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "ContentTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ContentTypeManagerDotNetAssemblyName", "ContentTypeManagerDotNetTypeName" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "ContentTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ContentTypeManagerDotNetAssemblyName", "ContentTypeManagerDotNetTypeName" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "ContentTypes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ContentTypeManagerDotNetAssemblyName", "ContentTypeManagerDotNetTypeName" },
                values: new object[] { null, null });
        }
    }
}
