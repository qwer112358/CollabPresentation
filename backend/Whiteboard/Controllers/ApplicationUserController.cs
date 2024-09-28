using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PresentationApp.Contracts;
using PresentationApp.Data;
using PresentationApp.Models;

namespace PresentationApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationUserController : ControllerBase
{
	private readonly ApplicationDbContext _dbContext;

	public ApplicationUserController(ApplicationDbContext dbContext) => _dbContext = dbContext;

	[HttpGet]
	public async Task<ActionResult<List<ApplicationUser>>> GetUsers()
	{
		var users = await _dbContext.ApplicationUsers.ToListAsync();
		if (users is null || users.Count == 0) return NotFound("No users found.");
		return Ok(ConvertToDto(users));
	}

	[HttpGet("{nickname}")]
	public async Task<ActionResult<ApplicationUser>> GetUser(string nickname)
	{

		var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Nickname == nickname);
		if (user is null) return NotFound("User not found.");
		return Ok(ConvertToDto(user));
	}

	[HttpPost]
	public async Task<ActionResult<ApplicationUser>> CreateUser(string nickname)
	{
		var user = new ApplicationUser { Id = Guid.NewGuid(), Nickname = nickname };
		try
		{
			_dbContext.ApplicationUsers.Add(user);
			await _dbContext.SaveChangesAsync();
		}
		catch (DbUpdateException)
		{
			ModelState.AddModelError("Nickname", "A user with this nickname already exists.");
			return BadRequest(ModelState);
		}
		return CreatedAtAction(nameof(GetUser), new { nickname = user.Nickname }, user);
	}


	[HttpDelete("{nickname}")]
	public async Task<IActionResult> DeleteUser(string nickname)
	{
		var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Nickname == nickname);
		if (user is null) return NotFound("User not found.");
		_dbContext.ApplicationUsers.Remove(user);
		await _dbContext.SaveChangesAsync();
		return NoContent();
	}

	private IEnumerable<ApplicationUserDto> ConvertToDto(IEnumerable<ApplicationUser> users) => users.Select(u => ConvertToDto(u));

	private ApplicationUserDto ConvertToDto(ApplicationUser user) => new ApplicationUserDto(user);
}

