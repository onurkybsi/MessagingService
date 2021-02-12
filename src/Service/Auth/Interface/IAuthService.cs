using System.Threading.Tasks;
using MessagingService.Model;

namespace MessagingService.Service
{
    public interface IAuthService
    {
        /// <param name="loginModel">Model with variables required for authentication</param>
        /// <summary>
        /// Using the injected IUserService, completes the login by parameter model. It returns the token generated for the user
        /// </summary>
        /// <returns>
        /// AuthResult that has generated token for user
        /// </returns>
        Task<AuthResult> Authenticate(LoginModel loginModel);
    }
}