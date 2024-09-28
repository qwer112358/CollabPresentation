using PresentationApp.Models;

namespace PresentationApp.Contracts;

public record SlideDto(string Content, int Order)
{
	public SlideDto() : this(string.Empty, 1) { }

	public SlideDto(Slide slide) : this(slide.Content, slide.Order) { }
}
