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
            if (MessageHubState.BlackList.Contains(Context.ConnectionId))
            {
                Context.Abort();
                return;
            }

            if (CheckSenderIsBlocked(sentMessage.ReceiverUser))
                return;

            var message = new Message { Content = sentMessage.Message, SenderUsername = Context.UserIdentifier, ReceiverUsername = sentMessage.ReceiverUser };

            await Clients.Users(Context.UserIdentifier, sentMessage.ReceiverUser).ReceiveMessage(message);

            await _messageService.SaveMessage(message);
        }

        private bool CheckSenderIsBlocked(string receiverUsername)
            => MessageHubState.ConnectedUsers.ContainsKey(receiverUsername) && MessageHubState.ConnectedUsers[receiverUsername].BlockedUsernames.Contains(Context.UserIdentifier);


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

            HandleConnectionContext(connectedUserName, connectionId);
            await base.OnConnectedAsync();
            await UpdateMessageHubStateAfterConnection(connectedUserName, connectionId);

            _logger.LogInformation($"{connectedUserName} connected with {connectionId} !");
        }

        private void HandleConnectionContext(string connectedUserName, string connectionId)
        {
            if (MessageHubState.ConnectedUsers.ContainsKey(connectedUserName))
                MessageHubState.BlackList.AddIfNoExist(MessageHubState.ConnectedUsers[connectedUserName].ConnectionId);
        }

        private async Task UpdateMessageHubStateAfterConnection(string connectedUserName, string connectionId)
        {
            ConnectedUserInfo connectedUserInfo = await _userService.GetConnectedUserInfo(connectedUserName);
            connectedUserInfo.ConnectionId = connectionId;

            MessageHubState.ConnectedUsers[connectedUserName] = connectedUserInfo;
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            string disconnectedUserName = Context.UserIdentifier;
            string disconnectedConnectionId = Context.ConnectionId;

            await base.OnDisconnectedAsync(exception);
            UpdateMessageHubStateAfterDisconnection(disconnectedUserName, disconnectedConnectionId);

            _logger.LogInformation($"{disconnectedUserName} disconnected with {disconnectedConnectionId} !");
        }

        private void UpdateMessageHubStateAfterDisconnection(string disconnectedUserName, string disconnectedConnectionId)
        {
            MessageHubState.ConnectedUsers.RemoveIfExist(disconnectedUserName);
            MessageHubState.BlackList.RemoveIfExist(disconnectedConnectionId);
        }
    }

    public static class MessageHubState
    {
        public static Dictionary<string, ConnectedUserInfo> ConnectedUsers = new Dictionary<string, ConnectedUserInfo>();

        /// <summary>
        /// When the second connection is connected with the existing username, the first connection is blacklisted.
        /// </summary>
        public static HashSet<string> BlackList { get; set; } = new HashSet<string>();
    }
}