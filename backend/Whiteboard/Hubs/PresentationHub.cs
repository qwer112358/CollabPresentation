using Microsoft.AspNetCore.SignalR;

namespace PresentationApp.Hubs;

public class PresentationHub : Hub
{
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
}
