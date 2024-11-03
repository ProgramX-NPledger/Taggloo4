using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunityContentItemContentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ContentTypes",
                columns: new[] { "Id", "ContentTypeKey", "ContentTypeManagerDotNetAssemblyName", "ContentTypeManagerDotNetTypeName", "Controller", "NamePlural", "NameSingular" },
                values: new object[] { 5, "CommunityContentItem", "Taggloo4.Translation", "Taggloo4.Translation.ContentTypes.CommunityContentItemContentTypeManager", "communitycontentitems", "Community Content Items", "Community Content Item" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ContentTypes",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
