namespace MessagingService.Infrastructure
{
    public class ProcessResult : IProcessResult
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
    }

    public class ProcessResult<T> : IProcessResult<T>
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public T ReturnObject { get; set; }
    }
}