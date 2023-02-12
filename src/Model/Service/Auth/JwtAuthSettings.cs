namespace MessagingService.Model {
  public class JwtAuthSettings {
    public string SecurityKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
  }
}