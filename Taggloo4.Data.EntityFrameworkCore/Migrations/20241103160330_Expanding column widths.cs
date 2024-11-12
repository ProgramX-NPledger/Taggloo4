using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Expandingcolumnwidths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SynopsisText",
                table: "CommunityContentItems",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "OriginalSynopsisHtml",
                table: "CommunityContentItems",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SynopsisText",
                table: "CommunityContentItems",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096);

            migrationBuilder.AlterColumn<string>(
                name: "OriginalSynopsisHtml",
                table: "CommunityContentItems",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096);
        }
    }
}
