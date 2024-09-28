namespace PresentationApp.Models;

public class ApplicationUser
{
	public Guid Id { get; set; }
	public string Nickname { get; set; }
	public List<PresentationUser> PresentationUsers { get; set; } = new();

}
