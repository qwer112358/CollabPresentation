using System.Text.Json.Serialization;

namespace PresentationApp.Models;

public class Slide
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid PresentationId { get; set; }
	
	[JsonIgnore]
	public Presentation? Presentation { get; set; }
	public List<Line> Lines { get; set; } = new List<Line>();
	public int Order { get; set; } = 1;

	public Slide() { }

	public Slide(Guid presentationId, List<Line> content, int order)
	{
		PresentationId = presentationId;
		Lines = content;
		Order = order;
	}
}
