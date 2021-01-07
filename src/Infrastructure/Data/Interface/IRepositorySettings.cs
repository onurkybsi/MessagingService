namespace MessagingService.Infrastructure
{
    public interface IRepositorySettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string CollectionName { get; set; }
    }
}