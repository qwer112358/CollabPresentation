using System.Text.Json.Serialization;

namespace PresentationApp.Models;

public class Slide
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid PresentationId { get; set; }
	
	[JsonIgnore]
	public Presentation? Presentation { get; set; }
	public string Content { get; set; }
	public int Order { get; set; } = 1;

	public Slide() { }

	public Slide(Guid presentationId, string content, int order)
	{
		PresentationId = presentationId;
		Content = content;
		Order = order;
	}
}
