using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslatorstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Translators",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DotNetFactoryType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DotNetFactoryAssembly = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translators", x => x.Key);
                });
            
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Translators");
        }
    }
}
