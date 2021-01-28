using System.Collections.Generic;
using MessagingService.Action;
using MessagingService.Model;

namespace MessagingService.Service
{
    public interface IMessageHubService : IBlockUser
    {
        HashSet<string> GetConnectedUsernames();
        void CreateMessageGroup(MessageGroupCreationContext context);
    }
}