using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class ChangetypeofImportIdtostring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportId",
                table: "Words");

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Words",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Words");

            migrationBuilder.AddColumn<Guid>(
                name: "ImportId",
                table: "Words",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
