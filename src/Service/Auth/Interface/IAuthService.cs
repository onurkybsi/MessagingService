using System.Threading.Tasks;
using MessagingService.Model;

namespace MessagingService.Service
{
    public interface IAuthService
    {
        Task<AuthResult> Authenticate(ILoginModel login);
    }
}