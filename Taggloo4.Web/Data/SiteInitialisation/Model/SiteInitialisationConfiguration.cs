

namespace Taggloo4.Web.Data.SiteInitialisation.Model;

/// <summary>
/// Site Initialisation configuration, deserialised from the siteInitialisation.json file.
/// </summary>
public class SiteInitialisationConfiguration
{
    /// <summary>
    /// Users to create as part of site initialisation.
    /// </summary>
    public User[] Users { get; set; } = Array.Empty<User>();

    /// <summary>
    /// Languages to create as part of site initialisation.
    /// </summary>
    /// <remarks>
    /// There must be exactly two Languages specified, if any are provided.
    /// </remarks>
    public Language[] Languages { get; set; } = Array.Empty<Language>();

}