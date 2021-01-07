namespace MessagingService.Infrastructure
{
    public class AuthSettings
    {
        public byte[] SecurityKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}