namespace MessagingService.Infrastructure
{
    public class MongoDBSettings : IDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}