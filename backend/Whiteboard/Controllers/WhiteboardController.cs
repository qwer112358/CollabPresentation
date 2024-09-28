using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PresentationApp.Data;
using PresentationApp.Models;

namespace PresentationApp.Controllers
{
	/*[ApiController]
	[Route("api/[controller]")]
	public class WhiteboardController : ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;

		public WhiteboardController(ApplicationDbContext context)
		{
			_dbContext = context;
		}

		// Получение всех линий с доски
		[HttpGet]
		public async Task<IActionResult> GetWhiteboardLines()
		{
			var lines = await _dbContext.Lines.ToListAsync();
			return Ok(lines);
		}

		// Сохранение новых линий на доске
		[HttpPost]
		public async Task<IActionResult> SaveLine([FromBody] List<Line> lines)
		{
			if (lines == null || lines.Count == 0)
				return BadRequest("Invalid data.");

			await _dbContext.Lines.AddRangeAsync(lines);
			await _dbContext.SaveChangesAsync();
			return Ok();
		}
	}*/
}
