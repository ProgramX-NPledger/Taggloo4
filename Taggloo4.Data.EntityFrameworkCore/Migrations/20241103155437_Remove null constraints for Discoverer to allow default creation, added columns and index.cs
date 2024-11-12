using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class RemovenullconstraintsforDiscoverertoallowdefaultcreationaddedcolumnsandindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CommunityContentDiscovererDotNetTypeName",
                table: "CommunityContentDiscoverers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "CommunityContentDiscovererDotNetAssemblyName",
                table: "CommunityContentDiscoverers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CommunityContentDiscoverers",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "CommunityContentDiscoverers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContentDiscoverers_Name",
                table: "CommunityContentDiscoverers",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CommunityContentDiscoverers_Name",
                table: "CommunityContentDiscoverers");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CommunityContentDiscoverers");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "CommunityContentDiscoverers");

            migrationBuilder.AlterColumn<string>(
                name: "CommunityContentDiscovererDotNetTypeName",
                table: "CommunityContentDiscoverers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CommunityContentDiscovererDotNetAssemblyName",
                table: "CommunityContentDiscoverers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }
    }
}
