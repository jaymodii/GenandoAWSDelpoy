using Microsoft.AspNetCore.SignalR;

namespace Common.Hubs
{
    public class BroadcastHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("BroadcastMessage");
        }
    }
}
