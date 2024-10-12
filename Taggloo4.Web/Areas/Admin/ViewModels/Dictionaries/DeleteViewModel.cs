using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Taggloo4.Contract.Criteria;
using Taggloo4.Model;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Dictionaries;

/// <summary>
/// View-Model for the deleting a migration.
/// </summary>
public class DeleteViewModel
{
    /// <summary>
    /// Identifier for Dictionary.
    /// </summary>
    [Display(Name="ID")]
    public int Id { get; set; }

    /// <summary>
    /// Name of Dictionary.
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string Name { get; set; }
    
    /// <summary>
    /// Name of the Language within the Dictionary.
    /// </summary>
    [Display(Name = "Language")]
    public required string LanguageName { get; set; }
    
    /// <summary>
    /// Name of Content Type (plural).
    /// </summary>
    [Display(Name = "Content Type")]
    public string? ContentTypeNamePlural { get; set; }

    /// <summary>
    /// Whether the Dictionary has a correctly configured Dictionary Manager, which is required for deletion operations.
    /// </summary>
    public bool HasValidConfiguredDictionaryManager { get; set; }

    /// <summary>
    /// Random verification code generated to require users to re-enter and avoid erroneously deleting data.
    /// </summary>
    public string? VerificationCode { get; set; }
    
    /// <summary>
    /// Confirmed verification code by user.
    /// </summary>
    [Required]
    [MaxLength(32)]
    [Display(Name = "Verification Code")]
    public string? ConfirmVerificationCode { get; set; }

    /// <summary>
    /// Whether the deletion job was successfully submitted.
    /// </summary>
    public bool DeleteJobSubmittedSuccessfully { get; set; }
    
}