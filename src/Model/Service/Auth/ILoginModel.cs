using System.ComponentModel.DataAnnotations;

namespace MessagingService.Model
{
    public interface ILoginModel
    {
        [Required]
        string Username { get; set; }
        [Required]
        string Password { get; set; }
    }
}