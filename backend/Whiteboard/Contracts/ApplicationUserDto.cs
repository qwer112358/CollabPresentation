using PresentationApp.Models;

namespace PresentationApp.Contracts;

public record ApplicationUserDto(string Nickname)
{
	public ApplicationUserDto() : this(string.Empty) { }
	public ApplicationUserDto(ApplicationUser user) : this(user.Nickname) { }
}
