namespace Taggloo4.Web.Data.SiteInitialisation;

/// <summary>
/// Represents the status of the site. A value of 0 (Ready) indicates that the site is ready.
/// </summary>
[Flags]
public enum SiteReadiness
{
    /// <summary>
    /// Site is ready for use.
    /// </summary>
    Ready = 0,
    /// <summary>
    /// Site is missing one or more required Roles.
    /// </summary>
    MissingRoles = 1,
    /// <summary>
    /// Site is missing one or more required Users.
    /// </summary>
    MissingUsers = 2,
    /// <summary>
    /// Site has less than two Languages configured.
    /// </summary>
    MissingLanguages = 4,
    /// <summary>
    /// Site has more than two Languages configured.
    /// </summary>
    TooManyLanguages = 8
}