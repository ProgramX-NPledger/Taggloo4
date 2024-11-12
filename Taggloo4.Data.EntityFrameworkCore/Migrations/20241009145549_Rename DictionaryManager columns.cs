using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class RenameDictionaryManagercolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DictionaryManagerFactoryDotNetTypeName",
                table: "Dictionaries",
                newName: "DictionaryManagerDotNetTypeName");

            migrationBuilder.RenameColumn(
                name: "DictionaryManagerFactoryDotNetAssemblyName",
                table: "Dictionaries",
                newName: "DictionaryManagerDotNetAssemblyName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DictionaryManagerDotNetTypeName",
                table: "Dictionaries",
                newName: "DictionaryManagerFactoryDotNetTypeName");

            migrationBuilder.RenameColumn(
                name: "DictionaryManagerDotNetAssemblyName",
                table: "Dictionaries",
                newName: "DictionaryManagerFactoryDotNetAssemblyName");
        }
    }
}
