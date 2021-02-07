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
            => MessageHubState.BlockedUsersInfo.ContainsKey(receiverUsername) && MessageHubState.BlockedUsersInfo[receiverUsername].Contains(Context.UserIdentifier);

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
            string connectionId = Context.ConnectionId;

            await base.OnConnectedAsync();
            await UpdateMessageHubStateAfterConnection(connectedUserName, connectionId);

            _logger.LogInformation($"{connectedUserName} connected !");
        }

        private async Task UpdateMessageHubStateAfterConnection(string connectedUserName, string connectionId)
        {
            MessageHubState.ConnectedUsers.Add(connectedUserName, connectionId);

            HashSet<string> blockedUsers = await _userService.GetBlockedUsersOfUser(connectedUserName);
            MessageHubState.BlockedUsersInfo.Add(connectedUserName, blockedUsers);

            if (await _userService.IsAdmin(connectedUserName))
                MessageHubState.ConnectedAdminUsernames.Add(connectedUserName);
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            string disconnectedUserName = Context.UserIdentifier;

            await base.OnDisconnectedAsync(exception);
            UpdateMessageHubStateAfterDisconnection(disconnectedUserName);

            _logger.LogInformation($"{disconnectedUserName} disconnected !");
        }

        private void UpdateMessageHubStateAfterDisconnection(string disconnectedUserName)
        {
            MessageHubState.ConnectedUsers.Remove(disconnectedUserName);

            if (MessageHubState.ConnectedAdminUsernames.Contains(disconnectedUserName))
                MessageHubState.ConnectedAdminUsernames.Remove(disconnectedUserName);

            MessageHubState.BlockedUsersInfo.Remove(disconnectedUserName);
        }
    }

    public static class MessageHubState
    {
        public static Dictionary<string, string> ConnectedUsers = new Dictionary<string, string>();
        public static HashSet<string> ConnectedAdminUsernames = new HashSet<string>();
        public static Dictionary<string, HashSet<string>> BlockedUsersInfo = new Dictionary<string, HashSet<string>>();
    }
}