using CommandLine;
using Taggloo4Mgt.Importing;

namespace Taggloo4Mgt;

public class MgtUtility
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MgtUtility(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public int Run(string[] args)
    {
        try
        {
            CommandLine.Parser.Default.ParseArguments<ImportOptions>(args)
                .MapResult(
                    (ImportOptions options) =>
                    {
                        Importer importer = new Importer(options, _httpClientFactory);
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

        return -1;
    }
}