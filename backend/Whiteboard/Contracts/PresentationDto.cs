using PresentationApp.Models;

namespace PresentationApp.Contracts;

public record PresentationDto(string Title, string OwnerName, List<SlideDto> Slides)
{
	public PresentationDto() : this(string.Empty, string.Empty, new List<SlideDto>()) { }

	public PresentationDto(Presentation presentation) : 
		this(presentation.Title, presentation.OwnerName, presentation.Slides.Select(s => new SlideDto(s)).ToList()) { }
}
