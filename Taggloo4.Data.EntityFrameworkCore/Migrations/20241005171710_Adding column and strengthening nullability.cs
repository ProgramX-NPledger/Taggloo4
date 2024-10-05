using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Addingcolumnandstrengtheningnullability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ContentTypes",
                newName: "NameSingular");

            migrationBuilder.AddColumn<string>(
                name: "NamePlural",
                table: "ContentTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NamePlural",
                table: "ContentTypes");

            migrationBuilder.RenameColumn(
                name: "NameSingular",
                table: "ContentTypes",
                newName: "Name");
        }
    }
}
