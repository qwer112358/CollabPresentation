namespace PresentationApp.Models;

public class PresentationUser
{
	public Guid UserId { get; set; }
	public ApplicationUser User { get; set; }

	public Guid PresentationId { get; set; }
	public Presentation Presentation { get; set; }

	public UserRole Role { get; set; }
}
