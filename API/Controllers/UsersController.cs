using API.Data;
using API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/v4/[controller]")]
public class UsersController : ControllerBase
{
	private readonly DataContext _dataContext;

	public UsersController(DataContext dataContext)
	{
		_dataContext = dataContext;
	}

	[HttpGet]
	// TODO: Add parameters to allow filtering, paging, return 400 if bad request
	public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
	{
		List<AppUser> users = await _dataContext.Users.ToListAsync();
		return users;
	}

	[HttpGet("{id}")]
	// TODO: Return 404 if not found
	public async Task<ActionResult<AppUser?>> GetUser(int id)
	{
		AppUser? user = await _dataContext.Users.FindAsync(id);
		return user;
	}
}