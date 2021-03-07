using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessagingService.Hubs;
using MessagingService.Model;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MessagingService.Service
{
    public class MessageHubService : IMessageHubService
    {
        private readonly IUserService _userService;
        private readonly IHubContext<MessageHub> _messageHubContex;
        private readonly ILogger<MessageHubService> _logger;

        public MessageHubService(IUserService userService, IHubContext<MessageHub> messageHubContex, ILogger<MessageHubService> logger)
        {
            _userService = userService;
            _messageHubContex = messageHubContex;
            _logger = logger;
        }

        public void BlockUser(UserBlockingContext context)
        {
            bool isCurrentUserInHub = MessageHubState.ConnectedUsers.ContainsKey(context.CurrentUsername);
            if (isCurrentUserInHub)
                MessageHubState.ConnectedUsers[context.CurrentUsername].BlockedUsernames.Add(context.BlockUserRequest.BlockedUsername);
        }

        public List<string> GetConnectedUsernames()
            => MessageHubState.ConnectedUsers.Keys.ToList();

        public async Task SaveMessageGroup(MessageGroupSaveContext context)
        {
            if (string.IsNullOrEmpty(context.MessageGroupId))
                throw new System.Exception($"{nameof(context.MessageGroupId)} is null or empty");

            var userInHub = CheckUserExistInHub(context.TransactionType == TransactionType.Insert
                ? context.CreationContext.AdminUsername
                : context.UpdateContext.Username);

            if (!userInHub.ExistInGroup)
                return;

            if (context.TransactionType == TransactionType.Update && context.UpdateContext.UpdateType == MessageGroupUpdateType.EliminationFromGroup)
                await _messageHubContex.Groups.RemoveFromGroupAsync(userInHub.ConnectionId, context.MessageGroupId);
            else
                await _messageHubContex.Groups.AddToGroupAsync(userInHub.ConnectionId, context.MessageGroupId);

            _logger.LogInformation(GetSaveMessageGroupInfoMessage(context));
        }

        private (bool ExistInGroup, string ConnectionId) CheckUserExistInHub(string username)
        {
            ConnectedUserInfo userInfo;
            MessageHubState.ConnectedUsers.TryGetValue(username, out userInfo);

            bool userExistInHub = userInfo != null && !string.IsNullOrEmpty(userInfo.ConnectionId);

            return (userExistInHub, userInfo.ConnectionId);
        }

        private string GetSaveMessageGroupInfoMessage(MessageGroupSaveContext context)
        {
            string infoMessage = Constants.MessageHubService.ErrorMessages.SaveMessageGroupInfoMessageCanNotCreate;

            if (context.TransactionType == TransactionType.Insert)
            {
                infoMessage = string.Format("New message group created: {0} by: {1}", context.CreationContext.GroupName, context.CreationContext.AdminUsername);
            }
            else if (context.TransactionType == TransactionType.Update && context.UpdateContext.UpdateType == MessageGroupUpdateType.AdditionToGroup)
            {
                infoMessage = string.Format("User: {0} added to message group: {1}", context.UpdateContext.MessageGroupId, context.UpdateContext.Username);
            }
            else
            {
                infoMessage = string.Format("{0} deleted from {1} message group", context.UpdateContext.Username, context.UpdateContext.MessageGroupId);
            }
            return infoMessage;
        }
    }
}