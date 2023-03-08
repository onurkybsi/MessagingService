using System.Net;
using System.Threading.Tasks;
using MessagingService.Service.Auth;
using MessagingService.Service.User;
using Microsoft.AspNetCore.Mvc;

namespace MessagingService.Controllers {

  [Route("api/[controller]/[action]")]
  [ApiController]
  public class AuthControllerV2 : ControllerBase {

    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public AuthControllerV2(IUserService userService, IAuthService authService) {
      _userService = userService;
      _authService = authService;
    }

    [HttpPost]
    async Task<IActionResult> SignUp([FromBody] SignUpRequest request) {
      User signedUpUser = await _userService.SignUp(request);
      return StatusCode(((int)HttpStatusCode.Created), signedUpUser);
    }

    [HttpGet]
    async Task<IActionResult> Login([FromBody] LoginRequest request) {
      LoginResult loginResult = await _authService.Authenticate(request);
      return Ok(loginResult);
    }

  }

}