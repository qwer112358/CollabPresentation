using PresentationApp.Models;

namespace PresentationApp.Contracts;

public record SlideDto(List<Line> Lines, int Order)
{
	public SlideDto() : this(new List<Line>(), 1) { }

	public SlideDto(Slide slide) : this(slide.Lines, slide.Order) { }
}
