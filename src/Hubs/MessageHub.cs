using System;
using System.Threading.Tasks;
using MessagingService.Infrastructure;
using MessagingService.Model;
using MessagingService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MessagingService.Hubs {
  [Authorize]
  public class MessageHub : Hub<IMessageClient> {
    private readonly ILogger<MessageHub> _logger;
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;

    public MessageHub(ILogger<MessageHub> logger, IMessageService messageService, IUserService userService) {
      _logger = logger;
      _messageService = messageService;
      _userService = userService;
    }

    public async Task SendPrivateMessage(SentMesage sentMessage) {
      ExecuteBeforeSendPrivateMessage(sentMessage);

      var message = new Message { Content = sentMessage.Message, SenderUsername = Context.UserIdentifier, ReceiverUsername = sentMessage.ReceiverUser };

      await Clients.Users(Context.UserIdentifier, sentMessage.ReceiverUser).ReceiveMessage(message);

      await ExecuteAfterSendPrivateMessage(message);
    }

    private void ExecuteBeforeSendPrivateMessage(SentMesage sentMessage) {
      if (MessageHubState.BlackList.Contains(Context.ConnectionId)) {
        Context.Abort();
        return;
      }

      if (CheckSenderIsBlocked(sentMessage.ReceiverUser))
        return;
    }

    private async Task ExecuteAfterSendPrivateMessage(Message message) {
      await _messageService.SaveMessage(message);
    }

    private bool CheckSenderIsBlocked(string receiverUsername)
        => MessageHubState.ConnectedUsers.ContainsKey(receiverUsername) && MessageHubState.ConnectedUsers[receiverUsername].BlockedUsernames.Contains(Context.UserIdentifier);

    public async override Task OnConnectedAsync() {
      string connectedUserName = Context.UserIdentifier;
      string connectionId = Context.ConnectionId;

      ExecuteBeforeOnConnectedAsync(connectedUserName, connectionId);
      await base.OnConnectedAsync();
      await ExecuteAfterOnConnectedAsync(connectedUserName, connectionId);
    }

    private void ExecuteBeforeOnConnectedAsync(string connectedUserName, string connectionId) {
      if (MessageHubState.ConnectedUsers.ContainsKey(connectedUserName))
        MessageHubState.BlackList.AddIfNoExist(MessageHubState.ConnectedUsers[connectedUserName].ConnectionId);
    }

    private async Task ExecuteAfterOnConnectedAsync(string connectedUserName, string connectionId) {
      ConnectedUserInfo connectedUserInfo = await _userService.GetConnectedUserInfo(connectedUserName);
      connectedUserInfo.ConnectionId = connectionId;

      MessageHubState.ConnectedUsers[connectedUserName] = connectedUserInfo;

      _logger.LogInformation($"{connectedUserName} connected with connection id: {connectionId} !");
    }

    public async override Task OnDisconnectedAsync(Exception exception) {
      string disconnectedUserName = Context.UserIdentifier;
      string disconnectedConnectionId = Context.ConnectionId;

      await base.OnDisconnectedAsync(exception);
      ExecuteAfterOnDisconnectedAsync(disconnectedUserName, disconnectedConnectionId);
    }

    private void ExecuteAfterOnDisconnectedAsync(string disconnectedUserName, string disconnectedConnectionId) {
      MessageHubState.ConnectedUsers.RemoveIfExist(disconnectedUserName);
      MessageHubState.BlackList.RemoveIfExist(disconnectedConnectionId);

      _logger.LogInformation($"{disconnectedUserName} disconnected with connection id: {disconnectedConnectionId} !");
    }
  }
}