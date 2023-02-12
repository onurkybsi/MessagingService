namespace MessagingService.Infrastructure {
  public interface IRepositorySettings<T> where T : IDatabaseSettings {
    T DatabaseSettings { get; set; }
  }
}