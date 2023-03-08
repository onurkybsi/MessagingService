namespace MessagingService.Service.Auth {

  public class JwtAuthConfiguration {

    public string SecurityKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }

  }

}