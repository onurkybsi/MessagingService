namespace MessagingService.Model {
  public class LoginModel {
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } = Constants.MessageHub.Role.User;
  }
}