using System.ComponentModel.DataAnnotations;

namespace MessagingService.Model
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string Role { get; set; } = Constants.MessageHub.Role.User;
    }
}