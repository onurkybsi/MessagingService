namespace MessagingService.Model
{
    public class AuthResult
    {
        public bool IsAuthenticated { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
    }
}