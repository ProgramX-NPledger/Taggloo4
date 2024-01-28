using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Represents a base class for API Controllers to inherit from, gaining access to a number of helper functions.
/// </summary>
[ApiController]
[Route("api/v4/[controller]")]
public class BaseApiController : ControllerBase
{
	/// <summary>
	/// Constant representing the JSON MIME type.
	/// </summary>
	protected const string JSON_MIME_TYPE = "application/json";
	
	/// <summary>
	/// Returns the base API path, including version suffix, for the current hosting platform.
	/// </summary>
	/// <returns>The base API path, including version suffix, eg. <c>http://taggloo.im/api/v4</c>.</returns>
	protected string GetBaseApiPath()
	{
		return $"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/v4";
	}

	/// <summary>
	/// Gets the IP address of the requesting host.
	/// </summary>
	/// <returns>The IP address of the requesting host.</returns>
	protected string GetRemoteHostAddress()
	{
		return Request.HttpContext.Connection.RemoteIpAddress!.ToString();
	}

	/// <summary>
	/// Gets the userName of the currently authenticated user.
	/// </summary>
	/// <returns>The userName of the currently authenticated user, or <c>null</c> if no user is currently authenticated.</returns>
	protected string? GetCurrentUserName()
	{
		return User.FindFirstValue(ClaimTypes.NameIdentifier);
	}

	/// <summary>
	/// Asserts that the request is respecting API user constraints.
	/// </summary>
	/// <param name="itemCount">Requested number of items requested.</param>
	/// <exception cref="BadHttpRequestException">Request breaches API constraints.</exception>
	protected void AssertApiConstraints(int itemCount)
	{
		if (itemCount > Defaults.MaximumPermittedItemsPerRequest)
			throw new BadHttpRequestException($"Requested item count exceeds maximum permitted value of {Defaults.MaximumPermittedItemsPerRequest}");
	}
}