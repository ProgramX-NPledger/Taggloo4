using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddSerilogtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sql = @"CREATE TABLE [Serilog] 
                            (
                                [Id] [int] IDENTITY(1,1) NOT NULL,
                                [Message] [nvarchar](max) NULL,
                                [MessageTemplate] [nvarchar](max) NULL,
                                [Level] [nvarchar](max) NULL,
                                [TimeStamp] [datetime] NULL,
                                [Exception] [nvarchar](max) NULL,
                                [Properties] [nvarchar](max) NULL,
                                CONSTRAINT [PK_Serilog] PRIMARY KEY CLUSTERED 
                                (
                                    [Id] ASC
                                )
                                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) 
                                ON [PRIMARY]
                            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Serilog");
        }
    }
}
