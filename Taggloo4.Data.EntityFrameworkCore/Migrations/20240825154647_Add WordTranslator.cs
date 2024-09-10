using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddWordTranslator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Translators",
                columns: new[] { "Key", "CreatedAt", "DotNetFactoryAssembly", "DotNetFactoryType", "IsEnabled" },
                values: new object[] { "WordTranslator", new DateTime(2024, 8, 25, 16, 46, 46, 775, DateTimeKind.Local).AddTicks(8609), "Taggloo4.Web", "Taggloo4.Web.Translation.Translators.Factories.WordTranslatorFactory", true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Translators",
                keyColumn: "Key",
                keyValue: "WordTranslator");
        }
    }
}
