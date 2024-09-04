using CommandLine;

namespace Taggloo4Mgt.Importing;

[Verb("import", HelpText = "Import from a Taggloo v2 SQL Server data source")]
public class ImportOptions
{
	[Option(HelpText="Connection string of source MS SQL Server database", Required=true)]
	public required string SourceConnectionString { get; set; }
	
	[Option(HelpText="Username of Taggloo 4 Taggloo4.Web", Required=true)] 
	public required string UserName { get; set; }
	
	[Option(HelpText = "Password of Taggloo 4 Taggloo4.Web", Required=true)]
	public required string Password { get; set; }
	
	[Option(HelpText="Url of Taggloo 4 Taggloo4.Web",Required = true)]
	public required string Url { get; set; }

	[Option(HelpText = "Log processing")]
	public bool Log { get; set; } = false;

	// [Option('m',HelpText = "Maximum items (per type) to import per type")]
	// public int? MaxItemsPerType { get; set; }

	// [Option(HelpText = "Delete all Dictionaries for detected Languages before importing")]
	// public bool ResetAllDictionaries { get; set; } = false;
	
	[Option(HelpText = "List of import types (eg. \"Words\", \"Phrases\"",Separator = ',')]
	public IEnumerable<string>? ImportTypes { get; set; }

}