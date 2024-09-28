using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PresentationApp.Contracts;
using PresentationApp.Data;
using PresentationApp.Models;

namespace PresentationApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PresentationUserController : ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;

		public PresentationUserController(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpPost("{presentationId:guid}")]
		public async Task<IActionResult> CreatePresentationUser(Guid presentationId, [FromBody] PresentationUserDto userDto)
		{
			// Найти презентацию с её пользователями
			var presentation = await _dbContext.Presentations
				.Include(p => p.PresentationUsers)
				.ThenInclude(pu => pu.User)  // Включаем пользователей
				.FirstOrDefaultAsync(p => p.Id == presentationId);

			if (presentation == null) return NotFound("Presentation not found.");

			// Найти пользователя по Nickname
			var existingUser = await _dbContext.ApplicationUsers
				.FirstOrDefaultAsync(u => u.Nickname == userDto.Nickname);

			if (existingUser == null)
			{
				// Если пользователь не найден, создать нового
				existingUser = new ApplicationUser { Id = Guid.NewGuid(), Nickname = userDto.Nickname };
				_dbContext.ApplicationUsers.Add(existingUser);
				await _dbContext.SaveChangesAsync(); // Сохраняем нового пользователя
			}

			// Проверить, существует ли уже связь PresentationUser
			var existingPresentationUser = presentation.PresentationUsers
				.FirstOrDefault(pu => pu.UserId == existingUser.Id);

			if (existingPresentationUser != null)
			{
				// Если пользователь уже есть в презентации, возвращаем его
				return Conflict("User is already a member of this presentation.");
			}

			// Если связи нет, добавляем нового PresentationUser
			var newPresentationUser = new PresentationUser
			{
				UserId = existingUser.Id,
				User = existingUser,
				PresentationId = presentation.Id,
				Role = userDto.Role
			};
			presentation.PresentationUsers.Add(newPresentationUser);

			// Добавляем в контекст и сохраняем
			_dbContext.PresentationUsers.Add(newPresentationUser);
			await _dbContext.SaveChangesAsync();

			return Ok(new PresentationUserDto(existingUser.Nickname, newPresentationUser.Role));
		}
	}
}
