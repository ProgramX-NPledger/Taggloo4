﻿using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Web.ViewModels;

/// <summary>
/// View Model for logging in to site.
/// </summary>
public class LoginViewModel
{
    /// <summary>
    /// Users email address (which is their username).
    /// </summary>
    [Required]
    [Display(Name="Email address or User name")]
    public required string EmailOrUserName { get; set; }
    
    /// <summary>
    /// Password required to authenticate.
    /// </summary>
    [Required]
    [Display(Name="Password")]
    [DataType(DataType.Password)] public required string Password { get; set; }

    /// <summary>
    /// When set will drop a persistent cookie.
    /// </summary>
    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }

    /// <summary>
    /// <c>True</c> if the User has just been successfully logged out.
    /// </summary>
    public bool LogoutSuccessful { get; set; } = false;
}