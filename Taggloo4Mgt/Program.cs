

using System.Diagnostics;
using CommandLine;
using Taggloo4Mgt;

Parser.Default.ParseArguments<ImportOptions>(args)
	.MapResult(
		(ImportOptions importOptions) =>
		{
			Importer importer = new Importer(importOptions);
			try
			{
				return importer.Process();
			}
			catch (Exception? ex)
			{
				Exception? exPtr = ex;
				do
				{
					Console.Error.WriteLine($"{exPtr.Message}");
					exPtr = exPtr.InnerException;
				} while (exPtr!=null);
				return 2;
			}
		},
		errors=>1
	);

static int ImportOptions(ImportOptions options)
{
	
}

	