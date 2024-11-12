using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDictionaryManagercolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DictionaryManagerDotNetAssemblyName",
                table: "Dictionaries");

            migrationBuilder.DropColumn(
                name: "DictionaryManagerDotNetTypeName",
                table: "Dictionaries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DictionaryManagerDotNetAssemblyName",
                table: "Dictionaries",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DictionaryManagerDotNetTypeName",
                table: "Dictionaries",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
        }
    }
}
