namespace MessagingService.Infrastructure
{
    public class MongoDbSettings : IDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}