namespace Taggloo4.Dto;

/// <summary>
/// Model for changing a User's password.
/// </summary>
public class ChangePassword
{
    /// <summary>
    /// The User's current password.
    /// </summary>
    public required string CurrentPassword { get; set; }
    
    /// <summary>
    /// The User's new password.
    /// </summary>
    public required string NewPassword { get; set; }
    
}