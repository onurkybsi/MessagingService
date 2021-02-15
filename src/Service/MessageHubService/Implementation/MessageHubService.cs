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
                await UpdateMessageHubGroup(context.CreationContext.AdminUsername, context.CreationContext.GroupName);
            else
                await UpdateMessageHubGroup(context.UpdateContext.AddedUsername, context.UpdateContext.GroupName);
        }

        private async Task UpdateMessageHubGroup(string addedUsername, string groupNameToAdd)
        {
            ConnectedUserInfo addedUserInfo;
            MessageHubState.ConnectedUsers.TryGetValue(addedUsername, out addedUserInfo);

            bool addedUserExistInHub = addedUserInfo != null && !string.IsNullOrEmpty(addedUserInfo.ConnectionId);
            if (addedUserExistInHub)
                await _messageHubContex.Groups.AddToGroupAsync(addedUserInfo.ConnectionId, groupNameToAdd);
        }

        public bool MustBeExecuteFirst => false;
    }
}