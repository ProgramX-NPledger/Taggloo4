using Microsoft.Extensions.FileProviders;

namespace Taggloo4.Web.RazorViewRendering;

/// <summary>
/// Implementation of <seealso cref="IWebHostEnvironment"/>, providing contextual data for the web host.
/// </summary>
public sealed class WebHostEnvironment : IWebHostEnvironment
{
    /// <summary>
    /// Name of the application.
    /// </summary>
    public string ApplicationName { get; set; } = default!;
    
    /// <summary>
    /// File provider for the web site location.
    /// </summary>
    public IFileProvider ContentRootFileProvider { get; set; } = default!;
    
    /// <summary>
    /// Physical file path of the web site.
    /// </summary>
    public string ContentRootPath { get; set; } = default!;
    
    /// <summary>
    /// Name of the environment.
    /// </summary>
    public string EnvironmentName { get; set; } = default!;

    /// <summary>
    /// Physical file path of the web content (which is where the <c>wwwroot</c> content is held).
    /// </summary>
    public string WebRootPath { get; set; } = default!;
  
    /// <summary>
    /// File provider for the web content (which is where the <c>wwwroot</c> content is held).
    /// </summary>
    public IFileProvider WebRootFileProvider { get; set; } = default!;
};