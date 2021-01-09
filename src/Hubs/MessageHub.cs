using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessagingService.Model;
using MessagingService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MessagingService.Hubs
{
    [Authorize]
    public class MessageHub : Hub<IMessageClient>
    {
        private readonly ILogger<MessageHub> _logger;
        private readonly IMessageService _messageService;

        public MessageHub(ILogger<MessageHub> logger, IMessageService messageService)
        {
            _logger = logger;
            _messageService = messageService;
        }

        public async Task SendMessageToAllUser(SentMesage sentMesage)
        {
            var message = new Message { MessageContent = sentMesage.Message, SenderUsername = Context.UserIdentifier };

            await Clients.All.ReceiveMessage(message);

            await _messageService.SaveMessage(message);
        }

        public async Task SendPrivateMessage(SentMesage sentMesage)
        {
            var message = new Message { MessageContent = sentMesage.Message, SenderUsername = Context.UserIdentifier, ReceiverUsername = sentMesage.ReceiverUser };

            await Clients.Users(Context.UserIdentifier, sentMesage.ReceiverUser).ReceiveMessage(message);

            await _messageService.SaveMessage(message);
        }

        public async override Task OnConnectedAsync()
        {
            MessageHubState.Usernames.Add(Context.UserIdentifier);
            await Clients.All.HandleConnectedUserChange(Context.UserIdentifier);
            _logger.LogInformation($"{Context.UserIdentifier} connected !");

            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            MessageHubState.Usernames.Remove(Context.UserIdentifier);
            await Clients.All.HandleConnectedUserChange(Context.UserIdentifier);
            _logger.LogInformation($"{Context.UserIdentifier} disconnected !");

            await base.OnDisconnectedAsync(exception);
        }
    }

    public static class MessageHubState
    {
        public static List<string> Usernames = new List<string>();
    }
}