using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedredundantcolumnsfromTranslatortable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Translators");

            migrationBuilder.DropColumn(
                name: "DotNetFactoryAssembly",
                table: "Translators");

            migrationBuilder.DropColumn(
                name: "DotNetFactoryType",
                table: "Translators");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Translators",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DotNetFactoryAssembly",
                table: "Translators",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DotNetFactoryType",
                table: "Translators",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Translators",
                columns: new[] { "Key", "CreatedAt", "DotNetFactoryAssembly", "DotNetFactoryType", "IsEnabled" },
                values: new object[] { "WordTranslator", new DateTime(2024, 8, 25, 16, 46, 46, 775, DateTimeKind.Local).AddTicks(8609), "Taggloo4.Web", "WordTranslatorFactory", true });
        }
    }
}
