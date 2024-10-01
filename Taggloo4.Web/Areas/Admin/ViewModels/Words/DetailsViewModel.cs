using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Taggloo4.Contract.Criteria;
using Taggloo4.Model;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Words;

/// <summary>
/// View-Model for the Word primary page.
/// </summary>
public class DetailsViewModel
{
    /// <summary>
    /// Identifier for Word.
    /// </summary>
    [Display(Name="ID")]
    public int Id { get; set; }
    
    /// <summary>
    /// The Word.
    /// </summary>
    [Display(Name="Word")]
    public required string TheWord { get; set; }
    
    /// <summary>
    /// UserName of creator.
    /// </summary>
    [Display(Name="Created by")]
    public required string CreatedByUserName { get; set; }
    
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
    /// External identifier of Word, to facilitate integration with an external system.
    /// </summary>
    [Display(Name="External ID")]
    public string? ExternalId { get; set; }

    /// <summary>
    /// Identifier of Dictionary.
    /// </summary>
    [Display(Name="Dictionary ID")]
    public int DictionaryId { get; set; }
    
    /// <summary>
    /// Name of Dictionary.
    /// </summary>
    [Display(Name="Dictionary Name")]
    public required string DictionaryName { get; set; }
    
    /// <summary>
    /// IETF Language Tag.
    /// </summary>
    [Display(Name="IETF Language Tag")]
    public required string IetfLanguageTag { get; set; }
    
    /// <summary>
    /// Language Name.
    /// </summary>
    [Display(Name="Language")]
    public required string LanguageName { get; set; }
    
    
}