using System;
using MessagingService.Infrastructure;

namespace MessagingService.Model
{
    public class Message : MongoDBEntity
    {
        public string Content { get; set; }
        public string SenderUsername { get; set; }
        public string ReceiverUsername { get; set; }
        public DateTime TimeToSend { get; } = DateTime.Now;
    }
}