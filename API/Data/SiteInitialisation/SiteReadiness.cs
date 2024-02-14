namespace API.Data;

[Flags]
public enum SiteReadiness
{
    Ready = 0,
    MissingRoles = 1,
    MissingUsers = 2,
    MissingLanguages = 4,
    TooManyLanguages = 8
}