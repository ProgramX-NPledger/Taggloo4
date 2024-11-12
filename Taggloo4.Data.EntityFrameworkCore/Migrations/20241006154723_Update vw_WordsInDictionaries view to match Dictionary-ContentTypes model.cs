using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Updatevw_WordsInDictionariesviewtomatchDictionaryContentTypesmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(@"ALTER view [dbo].[vw_WordsInDictionaries] as
	select w.Id as WordId,w.TheWord,w.CreatedByUserName,w.CreatedAt,w.CreatedOn,w.ExternalId,
		d.Id as DictionaryId,d.Name as DictionaryName, ct.NamePlural as ContentTypeFriendlyName,
		l.IetfLanguageTag,l.Name as LanguageName,
		count(wip.id) as AppearsInPhrasesCount
	from Words w
	inner join Dictionaries d on d.Id=w.DictionaryId
	inner join ContentTypes ct on ct.Id=d.ContentTypeId
	inner join Languages l on l.IetfLanguageTag=d.IetfLanguageTag
	left outer join WordsInPhrases wip on wip.WordId=w.id
	group by w.Id,w.TheWord,w.CreatedByUserName,w.CreatedAt,w.CreatedOn,w.ExternalId,d.Id,d.Name,ct.NamePlural,l.IetfLanguageTag,l.Name
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
