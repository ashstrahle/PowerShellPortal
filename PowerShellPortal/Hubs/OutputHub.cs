using Microsoft.AspNetCore.SignalR;

namespace PowerShellPortal.Controllers
{
    public class OutputHub : Hub
    {
        public async Task Send(string message, string connID)
        {
            // Call the addNewMessageToPage method to update clients.
            // Clients.All.addNewMessageToPage(message);
            // Clients.Caller.addNewMessageToPage(message);
            await Clients.Client(connID).SendAsync("show", message);
            //await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async override Task<Task> OnConnectedAsync()
        {
            string name = Environment.UserName;

            await Groups.AddToGroupAsync(Context.ConnectionId, name);

            return base.OnConnectedAsync();
        } 
    }
}