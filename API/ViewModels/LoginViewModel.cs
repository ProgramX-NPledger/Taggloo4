using System.ComponentModel.DataAnnotations;

namespace API.ViewModels;

/// <summary>
/// View Model for logging in to site.
/// </summary>
public class LoginViewModel
{
    /// <summary>
    /// Users email address (which is their username).
    /// </summary>
    [Required]
    public required string EmailOrUserName { get; set; }
    
    /// <summary>
    /// Password required to authenticate.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    /// <summary>
    /// When set will drop a persistent cookie.
    /// </summary>
    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }
}