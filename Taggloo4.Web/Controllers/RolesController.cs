using System.Security.Cryptography;
using System.Text;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taggloo4.Dto;
using Taggloo4.Web.Model;

namespace Taggloo4.Web.Controllers;

/// <summary>
/// User operations. All methods require authorisation.
/// </summary>
[Authorize]
public class RolesController : BaseApiController
{
	private readonly RoleManager<AppRole> _roleManager;

	/// <summary>
	/// Constructor with injected parameters.
	/// </summary>
	/// <param name="roleManager">Implementation of <seealso cref="RoleManager{AppRole}"/></param>
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