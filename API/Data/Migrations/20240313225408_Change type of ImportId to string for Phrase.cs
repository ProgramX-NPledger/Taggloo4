using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangetypeofImportIdtostringforPhrase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportId",
                table: "Phrases");

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Phrases",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Phrases");

            migrationBuilder.AddColumn<Guid>(
                name: "ImportId",
                table: "Phrases",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
