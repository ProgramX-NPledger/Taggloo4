using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace API.RazorViewRendering;

/// <summary>
/// Renders a Razor view outside of an HTPT context.
/// https://medium.com/@serhiikokhan/razor-view-rendering-in-net-6-1470cd849388
/// </summary>
public sealed class RazorViewRenderer
{
    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Constructor with parameters.
    /// </summary>
    /// <param name="viewEngine"></param>
    /// <param name="tempDataProvider"></param>
    /// <param name="serviceProvider"></param>
    /// <exception cref="ArgumentNullException"></exception>
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

    /// <summary>
    /// Renders the specified View to a string.
    /// </summary>
    /// <param name="viewPath">Path of the View to compile.</param>
    /// <param name="model">View Model to provide to the View.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <typeparam name="T">Type of View Model.</typeparam>
    /// <returns>A string representing the compiled Razor View.</returns>
    /// <exception cref="ArgumentNullException">Thrown if a required argument is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the View cannot be found/compiled.</exception>
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