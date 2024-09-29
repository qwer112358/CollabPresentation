namespace PresentationApp.Models;

public class Presentation
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Title { get; set; }
	public string OwnerName { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public List<Slide> Slides { get; set; } = new();
	public List<PresentationUser> PresentationUsers { get; set; } = new();

	public Presentation() { }

	public Presentation(string title, string ownerName)
	{
		Title = title;
		OwnerName = ownerName;
		Slides.Add(new Slide(Id, new List<Line>(), 1));
		PresentationUsers = new List<PresentationUser>
		{
			new PresentationUser
			{
				User = new ApplicationUser { Id = Guid.NewGuid(), Nickname = ownerName },
				Role = UserRole.Owner
			}
		};
	}

	public void AddSlide() => Slides.Add(new Slide(Id, new List<Line>(), Slides.Count + 1));

	public void RemoveSlide(int slideIndex) => Slides.RemoveAt(slideIndex);

	public void AddUser(ApplicationUser user, UserRole role = UserRole.Viewer) =>
		PresentationUsers.Add(new PresentationUser { User = user, Role = role });
}

