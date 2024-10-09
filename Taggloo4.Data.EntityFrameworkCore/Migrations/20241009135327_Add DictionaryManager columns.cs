using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddDictionaryManagercolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DictionaryManagerFactoryDotNetAssemblyName",
                table: "Dictionaries",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DictionaryManagerFactoryDotNetTypeName",
                table: "Dictionaries",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DictionaryManagerFactoryDotNetAssemblyName",
                table: "Dictionaries");

            migrationBuilder.DropColumn(
                name: "DictionaryManagerFactoryDotNetTypeName",
                table: "Dictionaries");
        }
    }
}
