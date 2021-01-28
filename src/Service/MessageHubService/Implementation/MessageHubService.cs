using System.Collections.Generic;
using System.Linq;
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
            bool isCurrentUserInHub = MessageHubState.BlockedUsersInfo.ContainsKey(context.CurrentUsername);
            if (isCurrentUserInHub)
                MessageHubState.BlockedUsersInfo.Where(bi => bi.Key == context.CurrentUsername).First().Value.Add(context.BlockUserRequest.BlockedUsername);
        }

        public void CreateMessageGroup(MessageGroupCreationContext context)
        {
            throw new System.NotImplementedException();
        }

        public HashSet<string> GetConnectedUsernames()
            => MessageHubState.ConnectedUsernames;
    }
}