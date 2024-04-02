using System.ComponentModel.DataAnnotations;

namespace API.ViewModels;

/// <summary>
/// View Model for user registration.
/// </summary>
public class RegisterViewModel
{
    /// <summary>
    /// Email of registering user. This will be the user's username.
    /// </summary>
    [Required]
    [EmailAddress]
    public required string Email { get; set; } 
    
    /// <summary>
    /// Desired password of registering user.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; } 

    /// <summary>
    /// Confirmation of desired password of registering user.
    /// </summary>
    [DataType(DataType.Password)]
    [Display(Name ="Confirm Password")]
    [Compare("Password",ErrorMessage ="Password and confirmation password not match.")]
    public required string ConfirmPassword { get; set; }
    
}