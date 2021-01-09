using System;
using MessagingService.Infrastructure;

namespace MessagingService.Model
{
    public class Message : MongoDBEntity
    {
        public string MessageContent { get; set; }
        public string SenderUsername { get; set; }
        public string ReceiverUsername { get; set; } = MessageHubConstants.ALL_USERS;
        public DateTime TimeToSend { get; } = DateTime.Now;
    }
}