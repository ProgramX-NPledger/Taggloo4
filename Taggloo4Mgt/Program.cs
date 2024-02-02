

using System.Diagnostics;
using CommandLine;
using Taggloo4Mgt;

var verbs = GetVerbs();

Type[] GetVerbs()
{
	return new Type[]
	{
		typeof(ImportOptions)
	};
}

void ProcessError(object obj)
{
	int i = 5;
}

void ProcessCommandLine(object obj)
{
	switch (obj)
	{
		case ImportOptions:
			Importer importer = new Importer((ImportOptions)obj);
			try
			{
				importer.Process();
			}
			catch (Exception? ex)
			{
				Exception? exPtr = ex;
				do
				{
					Console.Error.WriteLine($"{exPtr.Message}");
					exPtr = exPtr.InnerException;
				} while (exPtr!=null);

				return;
			}
			break;
	}
}


Parser.Default.ParseArguments(args, verbs)
	.WithParsed(ProcessCommandLine)
	.WithNotParsed(ProcessError);




	