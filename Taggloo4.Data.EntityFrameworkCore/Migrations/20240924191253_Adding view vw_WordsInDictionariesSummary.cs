using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Addingviewvw_WordsInDictionariesSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
create view vw_WordsInDictionariesSummary as
select DictionaryId,d.Name as DictionaryName, count(w.id) as WordCount, max(w.CreatedAt) as LatestWordCreatedAt
	from Words w
	inner join Dictionaries d on d.Id=w.DictionaryId
	group by DictionaryId,d.name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view vw_WordsInDictionariesSummary");
        }
    }
}
