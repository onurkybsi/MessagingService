namespace MessagingService.Infrastructure
{
    public class MongoDbCollectionSettings : IRepositorySettings<MongoDbSettings>
    {
        public MongoDbSettings DatabaseSettings { get; set; }
        public string CollectionName { get; set; }
    }
}