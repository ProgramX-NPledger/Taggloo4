using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.PlatformAbstractions;

namespace API.RazorViewRendering;

public static class RazorViewRendererFactory
{
    public static RazorViewRenderer New(string contentRootPath, string webRootPath, string defaultApplicationName)
    {
        if (string.IsNullOrWhiteSpace(contentRootPath))
            throw new ArgumentNullException(nameof(contentRootPath));
        if (string.IsNullOrWhiteSpace(webRootPath))
            throw new ArgumentNullException(nameof(webRootPath));
        if (string.IsNullOrWhiteSpace(defaultApplicationName))
            throw new ArgumentNullException(nameof(defaultApplicationName));

        ServiceCollection services = new();
        ApplicationEnvironment? applicationEnvironment = PlatformServices.Default.Application;
        services.AddSingleton(applicationEnvironment);

        WebHostEnvironment environment = new()
        {
            ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? defaultApplicationName,
            ContentRootPath = contentRootPath,
            ContentRootFileProvider = new PhysicalFileProvider(contentRootPath),
            WebRootPath = webRootPath,
            WebRootFileProvider = new PhysicalFileProvider(webRootPath)
        };
        services.AddSingleton<IWebHostEnvironment>(environment);

        services.Configure<MvcRazorRuntimeCompilationOptions>(mrrco =>
            {
                mrrco.FileProviders.Clear();
                mrrco.FileProviders.Add(new PhysicalFileProvider(contentRootPath));
            }
        );
        services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.TryAddSingleton(new DiagnosticListener("Microsoft.AspNetCore"));
        services.TryAddSingleton<DiagnosticSource>(sp => sp.GetRequiredService<DiagnosticListener>());
        services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.TryAddSingleton<ConsolidatedAssemblyApplicationPartFactory>();
        services.AddLogging()
            .AddHttpContextAccessor()
            .AddMvcCore()
            .AddRazorPages()
            .AddRazorRuntimeCompilation();
        services.AddSingleton<RazorViewRenderer>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        return serviceProvider.GetRequiredService<RazorViewRenderer>();
    }
};