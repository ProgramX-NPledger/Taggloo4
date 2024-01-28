using System.Text;
using API.Contract;
using API.Middleware.Model;

namespace API.Middleware;


  /// <summary>
/// Middleware for Logging Request and Responses.
/// Remarks: Original code taken from https://exceptionnotfound.net/using-middleware-to-log-requests-and-responses-in-asp-net-core/
/// </summary>
public class HttpLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IApiLoggerService _apiLoggerService;

    public HttpLoggingMiddleware(RequestDelegate next, IApiLoggerService apiLoggerService)
    {
        _next = next;
        _apiLoggerService = apiLoggerService;
        
    }

    /// <summary>
    /// Invokes the middleware component.
    /// </summary>
    /// <param name="context">The <seealso cref="HttpContext"/> of the request/response.</param>
    public async Task Invoke(HttpContext context)
    {
        DateTime start = DateTime.Now;
        
        //Copy  pointer to the original response body stream
        var originalBodyStream = context.Response.Body;

        //Get incoming request
        var request = await GetRequestAsync(context.Request);

        //Create a new memory stream and use it for the temp reponse body
        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        //Continue down the Middleware pipeline
        await _next(context);

        //Format the response from the server
        var response = await GetResponseAsync(context.Response);
        //Log it

        TimeSpan time = DateTime.Now - start;

        _apiLoggerService.Log(request.IpAddress, request.RequestVerb, request.SafeUrl, response.StatusCode,
            response.ResponseText, time.TotalMilliseconds);

        //Copy the contents of the new memory stream, which contains the response to the original stream, which is then returned to the client.
        await responseBody.CopyToAsync(originalBodyStream);
    }


    private async Task<HttpRequestForLog> GetRequestAsync(HttpRequest request)
    {
        var body = request.Body;

        //Set the reader for the request back at the beginning of its stream.
        request.EnableBuffering();

        //Read request stream
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];

        //Copy into  buffer.
        var _ = await request.Body.ReadAsync(buffer, 0, buffer.Length);

        //Assign the read body back to the request body
        request.Body = body;

        return new HttpRequestForLog()
        {
            IpAddress = request.HttpContext.Connection.RemoteIpAddress!.ToString(),
            RequestVerb = request.Method,
            SafeUrl = $"{request.Scheme}://{request.Host}{request.Path}{GetSafeQueryString(request.QueryString)}"
        };
    }

    private string GetSafeQueryString(QueryString requestQueryString)
    {
        StringBuilder sb = new StringBuilder();
        if (requestQueryString.HasValue)
        {
            //sb.Append("?");
            string[] split = requestQueryString.Value.Split(new char[] { '&' });
            int i = 0;
            foreach (string key in split)
            {
                if (i > 0) sb.Append("&");
                if (key.ToLower().StartsWith("password="))
                {
                    // do not render
                }
                else
                {
                    sb.Append(key);
                }

                i++;
            }
        }

        return sb.ToString();
    }

    private async Task<HttpResponseForLog> GetResponseAsync(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        //Create stream reader to write entire stream
        var text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return new HttpResponseForLog()
        {
            StatusCode = response.StatusCode,
            ResponseText = ""
        };
    }
}
