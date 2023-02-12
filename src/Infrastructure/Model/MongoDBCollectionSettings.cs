using MongoDB.Driver;

namespace MessagingService.Infrastructure {
  public class MongoDBCollectionSettings : IRepositorySettings<MongoDBSettings> {
    public MongoDBSettings DatabaseSettings { get; set; }
    public string CollectionName { get; set; }
    public CreateCollectionOptions CreateCollectionOptions { get; set; }
  }
}