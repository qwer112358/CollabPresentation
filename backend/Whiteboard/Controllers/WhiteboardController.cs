using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PresentationApp.Data;
using PresentationApp.Models;

namespace PresentationApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WhiteboardController : ControllerBase
{
	private readonly ApplicationDbContext _context;

	public WhiteboardController(ApplicationDbContext context)
	{
		_context = context;
	}

	[HttpGet]
	public async Task<IActionResult> GetWhiteboardLines(string slideId)
	{
		var lines = await _context.Lines
			.Where(l => l.SlideId.ToString() == slideId)
			.ToListAsync();

		return Ok(lines);
	}


	[HttpPost]
	public async Task<IActionResult> SaveLine([FromBody] List<Line> lines)
	{
		if (lines == null || lines.Count == 0)
			return BadRequest("Invalid data.");

		await _context.Lines.AddRangeAsync(lines);
		await _context.SaveChangesAsync();
		return Ok();
	}
}
