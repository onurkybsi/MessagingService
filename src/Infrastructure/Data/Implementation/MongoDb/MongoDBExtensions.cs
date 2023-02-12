using MongoDB.Bson;
using MongoDB.Driver;

namespace MessagingService.Infrastructure {
  public static class MongoDBExtensions {
    public static IMongoCollection<T> CreateCollectionIfNotExists<T>(this IMongoDatabase mongoDatabase, string collectionName, CreateCollectionOptions createCollectionOptions) {
      bool collectionIsExists = mongoDatabase.ListCollectionNames(new ListCollectionNamesOptions { Filter = new BsonDocument("name", collectionName) }).Any();

      if (!collectionIsExists)
        mongoDatabase.CreateCollection(collectionName, createCollectionOptions);

      return mongoDatabase.GetCollection<T>(collectionName);
    }
  }
}