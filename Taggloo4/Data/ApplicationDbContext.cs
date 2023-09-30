using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Duende.IdentityServer.EntityFramework.Options;
using Taggloo4.Models;

namespace Taggloo4.Data;

public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
{
	public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
		: base(options, operationalStoreOptions)
	{
	}
}