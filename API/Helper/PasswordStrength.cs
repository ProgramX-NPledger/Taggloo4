namespace API.Helper;

public class PasswordStrength
{
	public static bool IsPasswordStrongEnough(string password)
	{
		return !string.IsNullOrWhiteSpace(password) &&
		       password.Length >= 8;
	}

	public static IEnumerable<string> FriendlyPasswordRequirements()
	{
		return new string[]
		{
			"Password is required",
			"At least 8 characters"
		};
	}
}