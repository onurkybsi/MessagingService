using System.Collections.Generic;

namespace MessagingService.Model
{
    public class ConnectedUserInfo
    {
        public string ConnectionId { get; set; }
        public HashSet<string> BlockedUsernames { get; set; }
    }
}