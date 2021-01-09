using MessagingService.Infrastructure;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MessagingService.Model
{
    public class Message : MongoDBEntity
    {
        public string MessageContent { get; set; }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SenderUserId { get; set; }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId ReceiverUserId { get; set; }
    }
}