using System.ComponentModel.DataAnnotations;

namespace MessagingService.Model
{
    public class LoginModel : ILoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}