using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Addingviewvw_WordsInDictionaries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(@"
create view vw_WordsInDictionaries as
select w.Id as WordId,w.TheWord,w.CreatedByUserName,w.CreatedAt,w.CreatedOn,w.ExternalId,
	d.Id as DictionaryId,d.Name as DictionaryName,d.ContentTypeFriendlyName,
	l.IetfLanguageTag,l.Name as LanguageName
	from Words w
	inner join Dictionaries d on d.Id=w.DictionaryId
	inner join Languages l on l.IetfLanguageTag=d.IetfLanguageTag");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("drop view vw_WordsInDictionaries");
        }
    }
}
