using System.Security.Cryptography;
using System.Text;
using API.Contract;
using API.Data;
using API.DTO;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

/// <summary>
/// User operations. All methods require authorisation.
/// </summary>
[Authorize]
public class RolesController : BaseApiController
{
	private readonly RoleManager<AppRole> _roleManager;

	public RolesController(RoleManager<AppRole> roleManager)
	{
		_roleManager = roleManager;
	}


	/// <summary>
	/// Retrieve Role details.
	/// </summary>
	/// <param name="roleName">Name of Role.</param>
	/// <returns>A Role.</returns>
	/// <response code="200">Role is found.</response>
	/// <response code="403">Not permitted.</response>
	/// <response code="404">Role is not found.</response>
	//[Authorize(Roles="administrator")]
	[HttpGet("{roleName}")]
	public async Task<ActionResult<GetRoleResult>> GetRole(string roleName)
	{
		string upperedRoleName = roleName.ToUpper();
		AppRole? role = await _roleManager.Roles.SingleOrDefaultAsync(q => q.NormalizedName == upperedRoleName);
		if (role == null) return NotFound();

		List<Link> links = new List<Link>
		{
			new Link()
			{
				Action = "get",
				Rel = "self",
				Types = new string[] { JSON_MIME_TYPE },
				HRef = $"{GetBaseApiPath()}/roles/{role.Name}" 
			}
		};

		return new GetRoleResult()
		{
			RoleName = role.Name ?? string.Empty,
			Links = links
		};
	}
	
}