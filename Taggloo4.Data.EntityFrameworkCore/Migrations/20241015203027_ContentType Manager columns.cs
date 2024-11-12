using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class ContentTypeManagercolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentTypeManagerDotNetAssemblyName",
                table: "ContentTypes",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentTypeManagerDotNetTypeName",
                table: "ContentTypes",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ContentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ContentTypeManagerDotNetAssemblyName", "ContentTypeManagerDotNetTypeName" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "ContentTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ContentTypeManagerDotNetAssemblyName", "ContentTypeManagerDotNetTypeName" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "ContentTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ContentTypeManagerDotNetAssemblyName", "ContentTypeManagerDotNetTypeName" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "ContentTypes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ContentTypeManagerDotNetAssemblyName", "ContentTypeManagerDotNetTypeName" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentTypeManagerDotNetAssemblyName",
                table: "ContentTypes");

            migrationBuilder.DropColumn(
                name: "ContentTypeManagerDotNetTypeName",
                table: "ContentTypes");
        }
    }
}
