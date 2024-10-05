using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddingContenTypestableandrelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserName",
                table: "Words",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentTypeId",
                table: "Dictionaries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContentTypeKey = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Controller = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_ContentTypeId",
                table: "Dictionaries",
                column: "ContentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dictionaries_ContentTypes_ContentTypeId",
                table: "Dictionaries",
                column: "ContentTypeId",
                principalTable: "ContentTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dictionaries_ContentTypes_ContentTypeId",
                table: "Dictionaries");

            migrationBuilder.DropTable(
                name: "ContentTypes");

            migrationBuilder.DropIndex(
                name: "IX_Dictionaries_ContentTypeId",
                table: "Dictionaries");

            migrationBuilder.DropColumn(
                name: "ContentTypeId",
                table: "Dictionaries");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserName",
                table: "Words",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
