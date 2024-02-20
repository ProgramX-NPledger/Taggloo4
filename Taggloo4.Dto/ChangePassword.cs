namespace Taggloo4.Dto;

/// <summary>
/// Model for changing a User's password.
/// </summary>
public class ChangePassword
{
    /// <summary>
    /// The User's current password.
    /// </summary>
    public string CurrentPassword { get; set; }
    
    /// <summary>
    /// The User's new password.
    /// </summary>
    public string NewPassword { get; set; }
    
}