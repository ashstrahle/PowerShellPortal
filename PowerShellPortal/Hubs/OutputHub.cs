using Microsoft.AspNetCore.SignalR;

namespace PowerShellPortal.Controllers
{
    public class OutputHub : Hub
    {
        public async Task Send(string message, string connID)
        {
            await Clients.Client(connID).SendAsync("show", message);
        }

        public async override Task<Task> OnConnectedAsync()
        {
            string name = Environment.UserName;

            await Groups.AddToGroupAsync(Context.ConnectionId, name);

            return base.OnConnectedAsync();
        } 
    }
}
