namespace API.Model;

public class AppUser
{
	public string UserName { get; set; }
	public int Id { get; set; }
	public byte[] PasswordHash { get; set; }
	public byte[] PasswordSalt { get; set; }
	
}