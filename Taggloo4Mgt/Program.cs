

using System.Diagnostics;
using CommandLine;
using Taggloo4Mgt;
using Taggloo4Mgt.Importing;

try
{
	CommandLine.Parser.Default.ParseArguments<ImportOptions>(args)
		.MapResult(
			(ImportOptions options) =>
			{
				Importer importer = new Importer(options);
				return importer.Process().Result;
			},
			errors => 1);

}
catch (Exception ex)
{
	Exception? exPtr = ex;
	do
	{
		Console.Error.WriteLine($"{exPtr.Message}");
		exPtr = exPtr.InnerException;
	} while (exPtr!=null);
}




	