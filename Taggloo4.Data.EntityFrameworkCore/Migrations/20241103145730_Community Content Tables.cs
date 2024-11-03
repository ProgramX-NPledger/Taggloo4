using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class CommunityContentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommunityContentDiscoverers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CommunityContentDiscovererDotNetAssemblyName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CommunityContentDiscovererDotNetTypeName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityContentDiscoverers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommunityContentCollections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SearchUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CommunityContentDiscovererId = table.Column<int>(type: "int", nullable: false),
                    PollFrequencyMins = table.Column<int>(type: "int", nullable: true),
                    LastPolledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPollingEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityContentCollections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityContentCollections_CommunityContentDiscoverers_CommunityContentDiscovererId",
                        column: x => x.CommunityContentDiscovererId,
                        principalTable: "CommunityContentDiscoverers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunityContentItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SynopsisText = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    AuthorUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SourceUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HashAlgorithm = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    OriginalSynopsisHtml = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    IsTruncated = table.Column<bool>(type: "bit", nullable: false),
                    RetrievedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DictionaryId = table.Column<int>(type: "int", nullable: false),
                    CommunityContentCollectionId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityContentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityContentItems_CommunityContentCollections_CommunityContentCollectionId",
                        column: x => x.CommunityContentCollectionId,
                        principalTable: "CommunityContentCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityContentItems_Dictionaries_DictionaryId",
                        column: x => x.DictionaryId,
                        principalTable: "Dictionaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContentCollections_CommunityContentDiscovererId",
                table: "CommunityContentCollections",
                column: "CommunityContentDiscovererId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContentItems_CommunityContentCollectionId",
                table: "CommunityContentItems",
                column: "CommunityContentCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityContentItems_DictionaryId",
                table: "CommunityContentItems",
                column: "DictionaryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommunityContentItems");

            migrationBuilder.DropTable(
                name: "CommunityContentCollections");

            migrationBuilder.DropTable(
                name: "CommunityContentDiscoverers");
        }
    }
}
