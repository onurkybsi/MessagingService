using System.Threading.Tasks;

namespace MessagingService.Service.Auth {

  public class JwtAuthService : IAuthService {

    private readonly JwtAuthConfiguration _configuration;
    private readonly IUserService _userService;

    public JwtAuthService(IUserService userService) {
      _userService = userService;
    }

    public Task<LoginResult> Authenticate(LoginRequest request) {
      throw new System.NotImplementedException();
    }

  }

}