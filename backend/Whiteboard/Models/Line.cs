using System.Text.Json.Serialization;

namespace PresentationApp.Models;

public class Line
{
	public int Id { get; set; }
	public string? Points { get; set; } 
	public string? Stroke { get; set; } 
	public string? Tool { get; set; }   // Тип инструмента: pencil, rect, circle, arrow
	public float? StartX { get; set; }
	public float? StartY { get; set; }
	public float? EndX { get; set; }
	public float? EndY { get; set; }
	public Guid SlideId { get; set; }
	public Slide? Slide { get; set; }
}
