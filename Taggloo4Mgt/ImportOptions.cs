using CommandLine;

namespace Taggloo4Mgt;

[Verb("import", HelpText = "Import from a Taggloo v2 SQL Server data source")]
public class ImportOptions
{
	[Option(HelpText="Connection string of source MS SQL Server database", Required=true)]
	public string SourceConnectionString { get; set; }
	
	[Option(HelpText="Username of Taggloo 4 API", Required=true)] 
	public string UserName { get; set; }
	
	[Option(HelpText = "Password of Taggloo 4 API", Required=true)]
	public string Password { get; set; }
	
	[Option(HelpText="Url of Taggloo 4 API",Required = true)]
	public string Url { get; set; }
	
	
	
}