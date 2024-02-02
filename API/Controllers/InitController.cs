using System.Security.Cryptography;
using System.Text;
using API.Contract;
using API.Data;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taggloo4.Dto;

namespace API.Controllers;

/// <summary>
/// Initialisation endpoint. This can only be called once and will invoke initialisation
/// of database and configuration.
/// </summary>
[Authorize]
public class InitController : BaseApiController
{

	/// <summary>
	///	Constructor with injected parameters.
	/// </summary>
	public LoginController()
	{
	}

	/// <summary>
	/// Begins initialisation of the Taggloo instance.
	/// </summary>
	/// <remarks>This method does not require authentication but may only be called once.</remarks>
	/// <param name="beginInitialisation">A <see cref="BeginInitialisation"/> representing initial configuration.</param>
	/// <returns>A <seealso cref="BeginInitialisationResult"/> indicating success and how to get status.</returns>
	/// <response code="202">Initialisation was accepted and has commenced.</response>
	/// <response code="423">Site initialisation is locked and may not be executed.</response>
	[AllowAnonymous]
	[HttpPost]
	public async Task<ActionResult<BeginInitialisationResult>> BeginInitialisation(BeginInitialisation beginInitialisation)
	{
		// lock against other initialisations
		
		// EF migration
		
		// configure
		
		
		return new BeginInitialisationResult()
		{
			
		};
	}
}