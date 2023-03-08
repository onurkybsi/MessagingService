using System.Threading.Tasks;

namespace MessagingService.Service.Auth {

  public interface IAuthService {

    Task<LoginResult> Authenticate(LoginRequest request);

  }

}