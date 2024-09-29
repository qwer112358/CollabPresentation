using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PresentationApp.Contracts;
using PresentationApp.Data;
using PresentationApp.Models;

namespace PresentationApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SlideController : ControllerBase
{
	private readonly ApplicationDbContext _dbContext;

	public SlideController(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	
	[HttpPost("{presentationId}")]
	public async Task<IActionResult> CreateSlide(Guid presentationId)
	{
		var presentation = await _dbContext.Presentations
			.Include(p => p.Slides)
			.FirstOrDefaultAsync(p => p.Id == presentationId);
		if (presentation == null) return NotFound("Presentation not found.");
		try
		{
			presentation.AddSlide();
			_dbContext.Slides.Add(presentation.Slides.Last());
			await _dbContext.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException)
		{
			return Conflict("The presentation was modified or deleted by another process.");
		}
		return Ok(presentation.Slides.Last());
	}

	[HttpPut("{id:guid}")]
	public async Task<IActionResult> UpdateSlide(Guid id, SlideDto slideDto)
	{
		var slide = await _dbContext.Slides.FindAsync(id);
		if (slide == null)
		{
			return NotFound("Slide not found for update.");
		}
		slide.Lines = slideDto.Lines;
		slide.Order = slideDto.Order;
		_dbContext.Entry(slide).State = EntityState.Modified;
		try
		{
			await _dbContext.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException)
		{
			return NotFound("Slide not found for update.");
		}

		return NoContent();
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> DeleteSlide(Guid id)
	{
		var slide = await _dbContext.Slides.FindAsync(id);
		if (slide is null) return NotFound("Slide not found.");
		_dbContext.Slides.Remove(slide);
		await _dbContext.SaveChangesAsync();
		return NoContent(); 
	}
}
