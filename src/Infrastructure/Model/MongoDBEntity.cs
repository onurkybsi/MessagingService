using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MessagingService.Infrastructure
{
    public class MongoDBEntity : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}