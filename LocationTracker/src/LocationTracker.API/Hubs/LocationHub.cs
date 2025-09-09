using Microsoft.AspNetCore.SignalR;

namespace LocationTracker.API.Hubs;

public class LocationHub : Hub
{
    public async Task SendLocation(string deviceId, double latitude, double longitude)
    {
        await Clients.All.SendAsync("ReceiveLocation", deviceId, latitude, longitude);
    }
    
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("ReceiveMessage", "Connected to location hub");
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await Clients.Caller.SendAsync("ReceiveMessage", "Disconnected from location hub");
        await base.OnDisconnectedAsync(exception);
    }
}