using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PresentationApp.Contracts;
using PresentationApp.Data;
using PresentationApp.Models;

namespace PresentationApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PresentationController : ControllerBase
{
	private readonly ApplicationDbContext _dbContext;

	public PresentationController(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	[HttpGet]
	public async Task<IActionResult> GetPresentations() =>
		Ok(await _dbContext.Presentations
			.Include(p => p.Slides)
			.Include(p => p.PresentationUsers)
			.ToListAsync());


	[HttpGet("{id}")]
	public async Task<IActionResult> GetPresentation(Guid id)
	{
		var presentation = await _dbContext.Presentations
			.Include(p => p.Slides)
			.Include(p => p.PresentationUsers)
			.FirstOrDefaultAsync(p => p.Id == id);
		return presentation is null ? NotFound() : Ok(new PresentationDto(presentation));
	}

	[HttpPost]
	public async Task<IActionResult> CreatePresentation([FromBody] PresentationDto presentationRequest)
	{
		var owner = await _dbContext.ApplicationUsers
			.FirstOrDefaultAsync(u => u.Nickname == presentationRequest.OwnerName);

		if (owner is null)
		{
			owner = new ApplicationUser { Id = Guid.NewGuid(), Nickname = presentationRequest.OwnerName };
			_dbContext.ApplicationUsers.Add(owner);
			await _dbContext.SaveChangesAsync(); 
		}

		var presentation = new Presentation(presentationRequest.Title, owner.Nickname)
		{
			PresentationUsers = new List<PresentationUser>
			{
				new PresentationUser
				{
					UserId = owner.Id,
					User = owner,
					Role = UserRole.Owner 
				}
			}
		};

		_dbContext.Presentations.Add(presentation);
		await _dbContext.SaveChangesAsync();
		return CreatedAtAction(nameof(GetPresentation), new { id = presentation.Id }, presentation);
	}


	[HttpPut("{id}")]
	public async Task<IActionResult> UpdatePresentation(Guid id, [FromBody] Presentation presentation)
	{
		if (id != presentation.Id) return BadRequest();
		_dbContext.Entry(presentation).State = EntityState.Modified;
		await _dbContext.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeletePresentation(Guid id)
	{
		var presentation = await _dbContext.Presentations.FindAsync(id);
		if (presentation is null) return NotFound();
		_dbContext.Presentations.Remove(presentation);
		await _dbContext.SaveChangesAsync();
		return NoContent();
	}

	[HttpGet("{presentationId}/slides")]
	public async Task<IActionResult> GetSlides(Guid presentationId)
	{
		var slides = await _dbContext.Presentations
			.Include(p => p.Slides)
			.FirstOrDefaultAsync(p => p.Id == presentationId);
		return slides is null ? NotFound() : Ok(slides.Slides.Select(s => new SlideDto(s)));
	}

	[HttpGet("{presentationId}/users")]
	public async Task<IActionResult> GetUsers(Guid presentationId)
	{
		var presentation = await _dbContext.Presentations
			.Include(p => p.PresentationUsers)
			.ThenInclude(pu => pu.User)
			.FirstOrDefaultAsync(p => p.Id == presentationId);
		return presentation is null ? NotFound() : Ok(presentation.PresentationUsers.Select(u => new PresentationUserDto(u)));
	}
}
