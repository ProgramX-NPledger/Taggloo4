using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace API.RazorViewRendering;

public sealed class RazorViewRenderer
{
    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public RazorViewRenderer(
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider
    )
    {
        _viewEngine = viewEngine ?? throw new ArgumentNullException(nameof(viewEngine));
        _tempDataProvider = tempDataProvider ?? throw new ArgumentNullException(nameof(tempDataProvider));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task<string> RenderAsync<T>(
        string viewPath,
        T model,
        CancellationToken cancellationToken = default
    ) where T : class
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(viewPath))
            throw new ArgumentNullException(nameof(viewPath));
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        DefaultHttpContext defaultHttpContext = new()
        {
            RequestServices = _serviceProvider
        };
        ActionContext actionContext = new(defaultHttpContext, new RouteData(), new ActionDescriptor());

        IView? view;
        ViewEngineResult viewEngineResult = _viewEngine.GetView(null, viewPath, false);
        if (viewEngineResult.Success)
            view = viewEngineResult.View;
        else
            throw new InvalidOperationException($"Unable to find View {viewPath}.");

        await using StringWriter stringWriter = new();
        ViewDataDictionary<T> viewDataDictionary = new(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
            Model = model
        };
        TempDataDictionary tempDataDictionary = new(actionContext.HttpContext, _tempDataProvider);
        ViewContext viewContext = new(
            actionContext,
            view,
            viewDataDictionary,
            tempDataDictionary,
            stringWriter,
            new HtmlHelperOptions()
        );
        await view.RenderAsync(viewContext);

        return stringWriter.ToString();
    }
}