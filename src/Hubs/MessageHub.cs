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
        private readonly IUserService _userService;

        public MessageHub(ILogger<MessageHub> logger, IMessageService messageService, IUserService userService)
        {
            _logger = logger;
            _messageService = messageService;
            _userService = userService;
        }

        public async Task SendPrivateMessage(SentMesage sentMessage)
        {
            if (CheckSenderIsBlocked(sentMessage.ReceiverUser))
                return;

            var message = new Message { Content = sentMessage.Message, SenderUsername = Context.UserIdentifier, ReceiverUsername = sentMessage.ReceiverUser };

            await Clients.Users(Context.UserIdentifier, sentMessage.ReceiverUser).ReceiveMessage(message);

            await _messageService.SaveMessage(message);
        }

        private bool CheckSenderIsBlocked(string receiverUsername)
            => MessageHubState.BlockedUsersInfo[receiverUsername].Contains(Context.UserIdentifier);

        [Authorize(Roles = Constants.MessageHub.Role.Admin)]
        public async Task SendMessageToAllUser(SentMesage sentMessage)
        {
            var message = new Message { Content = sentMessage.Message, SenderUsername = Context.UserIdentifier, ReceiverUsername = Constants.MessageHub.AllUsers };

            await Clients.All.ReceiveMessage(message);

            await _messageService.SaveMessage(message);
        }

        public async override Task OnConnectedAsync()
        {
            string connectedUserName = Context.UserIdentifier;

            await base.OnConnectedAsync();
            await UpdateMessageHubStateAfterConnection(connectedUserName);
        }

        private async Task UpdateMessageHubStateAfterConnection(string connectedUserName)
        {
            MessageHubState.ConnectedUsernames.Add(connectedUserName);
            _logger.LogInformation($"{connectedUserName} connected !");

            if (await _userService.IsAdmin(connectedUserName))
                MessageHubState.ConnectedAdminUsernames.Add(connectedUserName);

            HashSet<string> blockedUsers = await _userService.GetBlockedUsersOfUser(connectedUserName);
            MessageHubState.BlockedUsersInfo.Add(connectedUserName, blockedUsers);
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            string disconnectedUserName = Context.UserIdentifier;
            await base.OnDisconnectedAsync(exception);

            UpdateMessageHubStateAfterDisconnection(disconnectedUserName);
        }

        private void UpdateMessageHubStateAfterDisconnection(string disconnectedUserName)
        {
            MessageHubState.ConnectedUsernames.Remove(disconnectedUserName);
            _logger.LogInformation($"{disconnectedUserName} disconnected !");

            if (MessageHubState.ConnectedAdminUsernames.Contains(disconnectedUserName))
                MessageHubState.ConnectedAdminUsernames.Remove(disconnectedUserName);

            MessageHubState.BlockedUsersInfo.Remove(disconnectedUserName);
        }
    }

    public static class MessageHubState
    {
        public static HashSet<string> ConnectedUsernames = new HashSet<string>();
        public static HashSet<string> ConnectedAdminUsernames = new HashSet<string>();
        public static Dictionary<string, HashSet<string>> BlockedUsersInfo = new Dictionary<string, HashSet<string>>();
    }
}