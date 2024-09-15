using System.Net;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Taggloo4.Web.Hangfire;

public class RoleAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly IEnumerable<string> _roles;

    public RoleAuthorizationFilter(IEnumerable<string> roles)
    {
        _roles = roles;
    }
    
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