using System.Threading.Tasks;
using MessagingService.Model;
using MessagingService.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MessagingService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, IAuthService authService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _authService = authService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] SignInModel newUser)
        {
            string hashedPass = EncryptionHelper.CreateHashed(newUser.Password);

            var createdUser = new User
            {
                Username = newUser.Username,
                HashedPassword = hashedPass,
            };

            await _userService.CreateUser(createdUser);
            _logger.LogInformation($"{createdUser.Id}-{createdUser.Id} signed in!");

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn([FromBody] LoginModel login)
        {
            var loginResult = await _authService.Authenticate(login);

            if (loginResult.IsAuthenticated)
                _logger.LogInformation($"{login.Username} loged in!");


            return Ok(loginResult);
        }
    }
}