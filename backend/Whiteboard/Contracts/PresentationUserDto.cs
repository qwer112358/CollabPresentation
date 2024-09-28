using PresentationApp.Models;

namespace PresentationApp.Contracts;

public record PresentationUserDto(string Nickname, UserRole Role)
{
	public PresentationUserDto() : this(string.Empty, UserRole.Viewer) { }
	public PresentationUserDto(PresentationUser user) : this(user.User.Nickname, user.Role) { }
}
