using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImportIdtoWordmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ImportId",
                table: "Words",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportId",
                table: "Words");
        }
    }
}
