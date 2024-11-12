using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDictionariestouseContentTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update d
		            set d.ContentTypeId=ct.Id
		            from Dictionaries d
		            inner join ContentTypes ct on ct.ContentTypeKey=d.ContentTypeKey
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
