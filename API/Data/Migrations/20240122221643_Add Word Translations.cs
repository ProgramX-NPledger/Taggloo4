using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWordTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Words_TheWord",
                table: "Words");

            migrationBuilder.CreateTable(
                name: "WordTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FromWordId = table.Column<int>(type: "INTEGER", nullable: false),
                    ToWordId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DictionaryId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordTranslation_Dictionaries_DictionaryId",
                        column: x => x.DictionaryId,
                        principalTable: "Dictionaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordTranslation_Words_FromWordId",
                        column: x => x.FromWordId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordTranslation_Words_ToWordId",
                        column: x => x.ToWordId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WordTranslation_DictionaryId",
                table: "WordTranslation",
                column: "DictionaryId");

            migrationBuilder.CreateIndex(
                name: "IX_WordTranslation_FromWordId",
                table: "WordTranslation",
                column: "FromWordId");

            migrationBuilder.CreateIndex(
                name: "IX_WordTranslation_ToWordId",
                table: "WordTranslation",
                column: "ToWordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WordTranslation");

            migrationBuilder.CreateIndex(
                name: "IX_Words_TheWord",
                table: "Words",
                column: "TheWord",
                unique: true);
        }
    }
}
