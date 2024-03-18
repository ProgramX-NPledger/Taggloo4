

using System.Diagnostics;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Taggloo4Mgt;
using Taggloo4Mgt.Importing;

var builder = new HostBuilder()
	.ConfigureServices((hostContext, services) =>
	{
		services.AddHttpClient()
			.ConfigureHttpClientDefaults(c =>
			{
				c.ConfigurePrimaryHttpMessageHandler(serviceProvider =>
				{
					HttpClientHandler httpClientHandler = new HttpClientHandler();
#if DEBUG
					httpClientHandler.ServerCertificateCustomValidationCallback =
						HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
#endif
					httpClientHandler.MaxConnectionsPerServer = 1;
					return httpClientHandler;
				});
			});
		services.AddTransient<MgtUtility>();
	}).UseConsoleLifetime();
 
var host = builder.Build();
 
using (var serviceScope = host.Services.CreateScope())
{
	var services = serviceScope.ServiceProvider;
 
	try
	{
		MgtUtility myService = services.GetRequiredService<MgtUtility>();
		int result = await myService.Run(args);

		return result;
	}
	catch (Exception ex)
	{
		Console.WriteLine("Error Occured");
	}
}

return 0;



	