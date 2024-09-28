using System.Net;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Taggloo4.Web.Hangfire;

/// <summary>
/// Customised filter for authentication against Hangfire Dashboard for ASP.NET Roles.
/// </summary>
public class RoleAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly IEnumerable<string> _roles;

    /// <summary>
    /// Constructor for configuration of the <seealso cref="RoleAuthorizationFilter"/>.
    /// </summary>
    /// <param name="roles">List of permitted roles.</param>
    public RoleAuthorizationFilter(IEnumerable<string> roles)
    {
        _roles = roles;
    }
    
    /// <summary>
    /// Authorises the request.
    /// </summary>
    /// <param name="context">The <seealso cref="DashboardContext"/> for the request.</param>
    /// <returns><c>True</c> if authorisation is successful.</returns>
    public bool Authorize(DashboardContext context)
    {   
        // Attempt to authenticate against Identity.Application scheme.
        // This will attempt to authenticate using data in request, but doesn't send challenge.
        HttpContext? httpContext = context.GetHttpContext();
        if (httpContext == null) return false;
        
        AuthenticateResult result = httpContext.AuthenticateAsync("Identity.Application").Result;
        if (!result.Succeeded)
        {
            // Request was not authenticated
            return false;
        }

        var isAuthenticated = result.Principal?.Identity?.IsAuthenticated ?? false;
        if (isAuthenticated == false)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }

        foreach (string role in _roles)
        {
            if (result.Principal?.IsInRole(role) ?? false) return true;
        }
        return false;
    }
}