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

    public bool HasValidConfiguredDictionaryManager { get; set; }

    public string VerificationCode { get; set; }
    
    [Required]
    [MaxLength(32)]
    [Display(Name = "Verification Code")]
    public string ConfirmVerificationCode { get; set; }
}