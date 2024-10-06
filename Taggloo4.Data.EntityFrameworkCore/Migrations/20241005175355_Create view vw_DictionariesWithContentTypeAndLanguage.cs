using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Createviewvw_DictionariesWithContentTypeAndLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(@"create view vw_DictionariesWithContentTypeAndLanguage as
		select d.Id as DictionaryId, d.Name, d.Description, d.CreatedAt, d.CreatedByUserName, d.CreatedOn, d.SourceUrl,
			ct.Id as ContentTypeId, ct.ContentTypeKey, ct.Controller, ct.NamePlural, ct.NameSingular,
			l.IetfLanguageTag, l.Name as LanguageName
			from Dictionaries d
			inner join ContentTypes ct on ct.id=d.ContentTypeId
			inner join Languages l on l.IetfLanguageTag=d.IetfLanguageTag");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(@"drop view vw_DictionariesWithContentTypeAndLanguage");
        }
    }
}
