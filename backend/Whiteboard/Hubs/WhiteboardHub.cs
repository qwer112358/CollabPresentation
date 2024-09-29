using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PresentationApp.Data;

namespace PresentationApp.Hubs;

/*public class WhiteboardHub : Hub
{
	private readonly ApplicationDbContext _context;

	public WhiteboardHub(ApplicationDbContext context)
	{
		_context = context;
	}

	public override async Task OnConnectedAsync()
	{
		var lines = await _context.Lines.ToListAsync();
		await Clients.Caller.SendAsync("LoadPreviousDrawings", lines);
		await base.OnConnectedAsync();
	}

	public async Task SendDrawAction(string user, string actionData)
	{
		await Clients.Others.SendAsync("ReceiveDrawAction", user, actionData);
	}
}*/

public class WhiteboardHub : Hub
{
	private readonly ApplicationDbContext _context;

	public WhiteboardHub(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task JoinPresentation(string presentationId, string user)
	{
		await Groups.AddToGroupAsync(Context.ConnectionId, presentationId);
		await Clients.Group(presentationId).SendAsync("UserJoined", user);
	}

	public async Task LeavePresentation(string presentationId, string user)
	{
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, presentationId);
		await Clients.Group(presentationId).SendAsync("UserLeft", user);
	}

	public async Task UpdateSlide(string presentationId, string slideData)
	{
		await Clients.Group(presentationId).SendAsync("SlideUpdated", slideData);
	}

	public override async Task OnConnectedAsync()
	{
		var lines = await _context.Lines.ToListAsync();
		await Clients.Caller.SendAsync("LoadPreviousDrawings", lines);
		await base.OnConnectedAsync();
	}

	public async Task SendDrawAction(string user, string actionData)
	{
		await Clients.Others.SendAsync("ReceiveDrawAction", user, actionData);
	}
}

