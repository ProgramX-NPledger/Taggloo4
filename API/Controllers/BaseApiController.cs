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
	
}