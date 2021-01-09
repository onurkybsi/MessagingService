using System.Threading.Tasks;
using MessagingService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MessagingService.Hubs
{
    [Authorize]
    public class MessageHub : Hub<IMessageClient>
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.ReceiveMessage(message);
        }
    }
}