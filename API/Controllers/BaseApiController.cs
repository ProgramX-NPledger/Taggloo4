using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v4/[controller]")]
public class BaseApiController : ControllerBase
{
	protected const string JSON_MIME_TYPE = "application/json";
	
	protected string GetBaseApiPath()
	{
		return $"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/v4";
	}

	protected string GetRemoteHostAddress()
	{
		return Request.HttpContext.Connection.RemoteIpAddress!.ToString();
	}

	protected string GetCurrentUserName()
	{
		return User.FindFirstValue(ClaimTypes.NameIdentifier);
	}

	protected void AssertApiConstraints(int itemCount)
	{
		if (itemCount > Defaults.MaximumPermittedItemsPerRequest)
			throw new BadHttpRequestException($"Requested item count exceeds maximum permitted value of {Defaults.MaximumPermittedItemsPerRequest}");
	}
}