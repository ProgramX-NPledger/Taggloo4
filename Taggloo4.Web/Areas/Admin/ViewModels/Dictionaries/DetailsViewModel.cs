using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Taggloo4.Contract.Criteria;
using Taggloo4.Model;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Dictionaries;

/// <summary>
/// View-Model for the Word primary page.
/// </summary>
public class DetailsViewModel
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
    /// Description of Dictionary
    /// </summary>
    [MaxLength(1024)]
    public required string Description { get; set; }

    /// <summary>
    /// URL of source of Dictionary.
    /// </summary>
    [MaxLength(1024)]
    public required string SourceUrl { get; set; }

    /// <summary>
    /// IETF Language-tag of the Dictionary. This must be a valid Language.
    /// </summary>
    [Display(Name="IETF Language Tag")]
    public required string IetfLanguageTag { get; set; }
    
    /// <summary>
    /// Name of the Language within the Dictionary.
    /// </summary>
    [Display(Name = "Language")]
    public required string LanguageName { get; set; }
    
    /// <summary>
    /// UserName of creator.
    /// </summary>
    [Display(Name="Created by")]
    public string? CreatedByUserName { get; set; }
    
    /// <summary>
    /// Timestamp of creation.
    /// </summary>
    [Display(Name="Created at")]
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Host on which Word was created.
    /// </summary>
    [Display(Name="Created on")]
    public required string CreatedOn { get; set; }

    /// <summary>
    /// Disambiguating key of Content Type.
    /// </summary>
    [Display(Name = "Content Type key")]
    public string? ContentTypeKey { get; set; }

    /// <summary>
    /// Identifier of Content Type.
    /// </summary>
    [Display(Name = "Content Type Id")]
    public int? ContentTypeId { get; set; }

    /// <summary>
    /// Name of Content Type (singular).
    /// </summary>
    [Display(Name = "Name (singular)")]
    public string? ContentTypeNameSingular { get; set; }

    /// <summary>
    /// Name of Content Type (plural).
    /// </summary>
    [Display(Name = "Name (plural)")]
    public string? ContentTypeNamePlural { get; set; }

    /// <summary>
    /// ASP.NET Controller for Content Type.
    /// </summary>
    [Display(Name="ASP.NET API Controller")]
    public string? ContentTypeController { get; set; }

    /// <summary>
    /// Name of .NET assembly with implementation of <seealso cref="IDictionaryManagerFactory"/>.
    /// </summary>
    [Display(Name="Assembly")]
    public string? ContentTypeManagerDotNetAssemblyName { get; set; }

    /// <summary>
    /// Name of .NET type for implementation of <seealso cref="IDictionaryManagerFactory"/>.
    /// </summary>
    [Display(Name="Type")]
    public string? ContentTypeManagerDotNetTypeName { get; set; }

    /// <summary>
    /// Whether the user is permitted to Delete this Dictionary.
    /// </summary>
    public bool IsPermittedToDelete { get; set; } = false;

}