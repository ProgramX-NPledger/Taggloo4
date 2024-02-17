namespace API.Data.SiteInitialisation.Model;

/// <summary>
/// Represents a User to be created as part of Site Initialisation.
/// </summary>
public class User
{
    /// <summary>
    /// The username of the user to create.
    /// </summary>
    public required string UserName { get; set; }
    
    /// <summary>
    /// Comma-separated list of Roles to assign to the User.
    /// </summary>
    public required string AssignToRoles { get; set; }
    
    /// <summary>
    /// The user's password
    /// </summary>
    public required string Password { get; set; }
}