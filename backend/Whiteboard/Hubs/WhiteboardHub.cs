using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PresentationApp.Data;

namespace PresentationApp.Hubs;

public class WhiteboardHub : Hub
{
	private readonly ApplicationDbContext _context;

	public WhiteboardHub(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task JoinPresentation(string presentationId, string user)
	{
		var presentation = await _context.Presentations.FindAsync(Guid.Parse(presentationId));
		var appUser = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Nickname == user);
		presentation?.AddUser(appUser!, Models.UserRole.Editor);
		var userList = presentation?.PresentationUsers.Select(u => u.User.Nickname);
		await Groups.AddToGroupAsync(Context.ConnectionId, presentationId);
		await Clients.Group(presentationId).SendAsync("UserJoined", user);
	}

	public async Task LeavePresentation(string presentationId, string user)
	{
		var presentation = await _context.Presentations.FindAsync(Guid.Parse(presentationId));
		var appUser = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Nickname == user);
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, presentationId);
		await Clients.Group(presentationId).SendAsync("UserLeft", user);
	}

	public override async Task OnConnectedAsync()
	{
		string slideId = Context.GetHttpContext().Request.Query["slideId"];

		if (!string.IsNullOrEmpty(slideId))
		{
			var lines = await _context.Lines
				.Where(l => l.SlideId.ToString() == slideId)
				.ToListAsync();
			await Clients.Caller.SendAsync("LoadPreviousDrawings", lines);
		}
		await base.OnConnectedAsync();
	}


	public async Task SendDrawAction(string user, string actionData)
	{
		await Clients.Others.SendAsync("ReceiveDrawAction", user, actionData);
	}
}

