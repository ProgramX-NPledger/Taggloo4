using System.Text.Json;
using API.Data.Model;
using API.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Language = API.Data.Model.Language;

namespace API.Data;

public class Initialiser
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ILogger<Initialiser> _logger;
    private readonly string _contentRootPath;
    private readonly DataContext _dataContext;

    public Initialiser(UserManager<AppUser> userManager, 
        RoleManager<AppRole> roleManager,
        ILogger<Initialiser> logger,
        string contentRootPath,
        DataContext dataContext
        )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _contentRootPath = contentRootPath;
        _dataContext = dataContext;
    }
    

    public async Task Initialise()
    {
       string siteInitialisationFileName = Path.Combine(_contentRootPath, "siteInitialisation.json");
        if (File.Exists(siteInitialisationFileName))
        {
            SiteReadiness siteReadiness = await GetSiteStatus();
            if (siteReadiness!=SiteReadiness.Ready)
            {
                string siteInitialisationConfigurationText = File.ReadAllText(siteInitialisationFileName);
                SiteInitialisationConfiguration? siteInitialisationConfiguration;
                try
                {
                    siteInitialisationConfiguration = JsonSerializer.Deserialize<SiteInitialisationConfiguration>(
                        siteInitialisationConfigurationText);
                    AssertValidSiteInitialisationConfiguration(siteInitialisationConfiguration);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,$"Failed to read configuration from {siteInitialisationFileName}");
                    throw;
                }
                
                _logger.LogInformation($"Site Initialisation file '{siteInitialisationFileName} found and site is capable for initialisation.");

                await EnsureRolesAreSeeded();
                await EnsureUsersAreSeeded(siteInitialisationConfiguration.Users);
                await EnsureLanguagesAreConfigured(siteInitialisationConfiguration.Languages);
            }
            else
            {
                _logger.LogWarning($"Site Initialisation file '{siteInitialisationFileName} still exists but was ignored. You should delete this file. Site Readiness is {siteReadiness}");
            }
            
        }

        SiteReadiness finalSiteInitialisationStatus = await GetSiteStatus();
        if (finalSiteInitialisationStatus!=SiteReadiness.Ready)
            throw new InvalidOperationException(
                $"Taggloo site is not yet ready for start-up. Configure a site initialisation file ({siteInitialisationFileName}) to prepare the site");
    }

    private void AssertValidSiteInitialisationConfiguration(SiteInitialisationConfiguration? siteInitialisationConfiguration)
    {
        List<string> errors = new List<string>();
        foreach (User user in siteInitialisationConfiguration.Users)
        {
            if (string.IsNullOrWhiteSpace(user.UserName)) errors.Add($"User has empty UserName");
            if (string.IsNullOrWhiteSpace(user.Password)) errors.Add($"User {user.UserName} has empty Password");
            if (string.IsNullOrWhiteSpace(user.AssignToRoles)) errors.Add($"User {user.UserName} has empty AssignToRoles");
        }

        foreach (Language language in siteInitialisationConfiguration.Languages)
        {
            if (string.IsNullOrWhiteSpace(language.IetfLanguageTag)) errors.Add($"Language has empty IetfLanguageCode");
            if (string.IsNullOrWhiteSpace(language.Name)) errors.Add($"Language {language.IetfLanguageTag} has empty Name");
        }
        
        if (errors.Any())
        {
            throw new InvalidOperationException($"Site Initialisation is invalid: " +
                                                string.Join(Environment.NewLine, errors.ToArray()));
        }
    }

    private async Task EnsureLanguagesAreConfigured(Language[] languages)
    {
        if (!_dataContext.Languages.Any())
        {
            // create languages
            foreach (Language language in languages)
            {
                _logger?.LogInformation($"Seeding missing Language '{language.IetfLanguageTag}'");
                _dataContext.Languages.Add(new API.Model.Language()
                {
                    Name = language.Name,
                    IetfLanguageTag = language.IetfLanguageTag
                });
            }

            await _dataContext.SaveChangesAsync();
        }
        
    }

    private async Task EnsureUsersAreSeeded(User[] users)
    {
        
        foreach (User user in users)
        {
            if (await _userManager.Users.SingleOrDefaultAsync(q => q.UserName.Equals(user.UserName)) == null)
            {
                // user doesn't exist, so create it
                AppUser appUser = new AppUser()
                {
                    UserName = user.UserName
                };
                await _userManager.CreateAsync(appUser,user.Password);
                _logger?.LogInformation($"Seeding missing user '{user.UserName}' and adding to roles '{user.AssignToRoles}'");
                string[] assignToRoles = user.AssignToRoles.Split([',']);
                foreach (string role in assignToRoles)
                {
                    await _userManager.AddToRoleAsync(appUser, role);
                }

            }
        }
    }

    private async Task EnsureRolesAreSeeded()
    {
        IEnumerable<string> requiredRoles = GetRequiredRoles();

        foreach (string role in requiredRoles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                _logger?.LogInformation($"Seeding missing role '{role}'");
                await _roleManager.CreateAsync(new AppRole()
                {
                    Name = role
                });    
            }
        }
    }

    private IEnumerable<string> GetRequiredRoles()
    {
        return new []
        {
            "dataImporter",
            "dataExporter",
            "translator",
            "administrator"
        };
    }

    private IEnumerable<string> GetRequiredUsers()
    {
        return new[]
        {
            "administrator",
            "translator"
        };
    }


    public async Task<SiteReadiness> GetSiteStatus()
    {
        SiteReadiness siteReadiness = 0;
        
        // must have basic roles
        IEnumerable<string> requiredRoles = GetRequiredRoles();
        foreach (string role in requiredRoles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                siteReadiness |= SiteReadiness.MissingRoles;
            }
        }
        
        // must have basic user accounts
        IEnumerable<string> requiredUsers = GetRequiredUsers();
        foreach (string user in requiredUsers)
        {
            if (await _userManager.Users.SingleOrDefaultAsync(q => q.UserName == user)==null)
                siteReadiness |= SiteReadiness.MissingUsers;
        }
        
        // must have base configuration

        // languages (2 and only 2)
        if (_dataContext.Languages.Count() < 2) siteReadiness |= SiteReadiness.MissingLanguages;
        if (_dataContext.Languages.Count() > 2) siteReadiness |= SiteReadiness.TooManyLanguages;
        return siteReadiness;
    }

    
}