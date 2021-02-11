using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessagingService.Hubs;
using MessagingService.Model;
using Microsoft.AspNetCore.SignalR;

namespace MessagingService.Service
{
    public class MessageHubService : IMessageHubService
    {
        private readonly IUserService _userService;
        private readonly IHubContext<MessageHub> _messageHubContex;

        public MessageHubService(IUserService userService, IHubContext<MessageHub> messageHubContex)
        {
            _userService = userService;
            _messageHubContex = messageHubContex;
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
            if (context.SaveType == SaveType.Insert)
            {
                string adminConnectionId = MessageHubState.ConnectedUsers[context.CreationContext.AdminUsername]?.ConnectionId;
                if (!string.IsNullOrEmpty(adminConnectionId))
                    await _messageHubContex.Groups.AddToGroupAsync(adminConnectionId, context.CreationContext.GroupName);
            }
            else
            {
                string addedUserConnectionId = MessageHubState.ConnectedUsers[context.UpdateContext.AddedUsername]?.ConnectionId;
                if (!string.IsNullOrEmpty(addedUserConnectionId))
                    await _messageHubContex.Groups.AddToGroupAsync(addedUserConnectionId, context.CreationContext.GroupName);
            }
        }
    }
}