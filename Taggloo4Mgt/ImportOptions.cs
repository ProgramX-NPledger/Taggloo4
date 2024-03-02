﻿using CommandLine;

namespace Taggloo4Mgt;

[Verb("import", HelpText = "Import from a Taggloo v2 SQL Server data source")]
public class ImportOptions
{
	[Option(HelpText="Connection string of source MS SQL Server database", Required=true)]
	public required string SourceConnectionString { get; set; }
	
	[Option(HelpText="Username of Taggloo 4 API", Required=true)] 
	public required string UserName { get; set; }
	
	[Option(HelpText = "Password of Taggloo 4 API", Required=true)]
	public required string Password { get; set; }
	
	[Option(HelpText="Url of Taggloo 4 API",Required = true)]
	public required string Url { get; set; }

	[Option(HelpText = "Log processing")]
	public bool Log { get; set; } = false;

	[Option('m',HelpText = "Maximum Words to import per language")]
	public int? MaxWordsPerLanguage { get; set; }


}