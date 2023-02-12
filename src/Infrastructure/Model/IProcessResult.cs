namespace MessagingService.Infrastructure {
  public interface IProcessResult {
    bool IsSuccessful { get; set; }
    string Message { get; set; }
  }

  public interface IProcessResult<T> : IProcessResult {
    T ReturnObject { get; set; }
  }
}