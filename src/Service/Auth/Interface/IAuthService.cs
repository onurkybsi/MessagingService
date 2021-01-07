using MessagingService.Model;

namespace MessagingService.Service
{
    public interface IAuthService
    {
        AuthResult Authenticate(ILoginModel user);
    }
}