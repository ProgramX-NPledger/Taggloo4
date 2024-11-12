using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Updatevw_WOrdsInDictionariesSummaryviewtoreflectMMrelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER view [dbo].[vw_WordsInDictionariesSummary] as
select d.Id as DictionaryId,d.Name as DictionaryName, count(w.id) as WordCount, max(w.CreatedAt) as LatestWordCreatedAt
	from Words w
	inner join DictionaryWord dw on dw.WordsId=w.Id
	inner join Dictionaries d on d.Id=dw.DictionariesId
	group by d.Id,d.name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
create view vw_WordsInDictionariesSummary as
select DictionaryId,d.Name as DictionaryName, count(w.id) as WordCount, max(w.CreatedAt) as LatestWordCreatedAt
	from Words w
	inner join Dictionaries d on d.Id=w.DictionaryId
	group by DictionaryId,d.name");
        }
    }
}
