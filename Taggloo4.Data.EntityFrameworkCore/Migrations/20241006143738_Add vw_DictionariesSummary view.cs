using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Addvw_DictionariesSummaryview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create view vw_DictionariesSummary as 
	select coalesce(count(distinct IetfLanguageTag),0) as NumberOfLanguagesInDictionaries, 
		coalesce(count (Id),0) as NumberOfDictionaries, 
		coalesce(count(distinct ContentTypeId),0) as NumberOfContentTypes from Dictionaries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop view vw_DictionariesSummary");
        }
    }
}
