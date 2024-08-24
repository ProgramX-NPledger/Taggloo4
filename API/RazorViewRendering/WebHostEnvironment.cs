using Microsoft.Extensions.FileProviders;

namespace API.RazorViewRendering;

public sealed class WebHostEnvironment : IWebHostEnvironment
{
    public string ApplicationName { get; set; } = default!;
    public IFileProvider ContentRootFileProvider { get; set; } = default!;
    public string ContentRootPath { get; set; } = default!;
    public string EnvironmentName { get; set; } = default!;
    public string WebRootPath { get; set; } = default!;
    public IFileProvider WebRootFileProvider { get; set; } = default!;
};