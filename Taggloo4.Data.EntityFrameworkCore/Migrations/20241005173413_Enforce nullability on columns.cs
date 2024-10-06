using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Enforcenullabilityoncolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dictionaries_ContentTypes_ContentTypeId",
                table: "Dictionaries");

            migrationBuilder.DropColumn(
                name: "ContentTypeFriendlyName",
                table: "Dictionaries");

            migrationBuilder.DropColumn(
                name: "ContentTypeKey",
                table: "Dictionaries");

            migrationBuilder.DropColumn(
                name: "Controller",
                table: "Dictionaries");

            migrationBuilder.AlterColumn<int>(
                name: "ContentTypeId",
                table: "Dictionaries",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Dictionaries_ContentTypes_ContentTypeId",
                table: "Dictionaries",
                column: "ContentTypeId",
                principalTable: "ContentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dictionaries_ContentTypes_ContentTypeId",
                table: "Dictionaries");

            migrationBuilder.AlterColumn<int>(
                name: "ContentTypeId",
                table: "Dictionaries",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ContentTypeFriendlyName",
                table: "Dictionaries",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentTypeKey",
                table: "Dictionaries",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Controller",
                table: "Dictionaries",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Dictionaries_ContentTypes_ContentTypeId",
                table: "Dictionaries",
                column: "ContentTypeId",
                principalTable: "ContentTypes",
                principalColumn: "Id");
        }
    }
}
