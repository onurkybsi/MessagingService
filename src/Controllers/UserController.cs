using System.Linq;
using System.Security.Claims;
using MessagingService.Action;
using MessagingService.Model;
using MessagingService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MessagingService.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IBlockUserAction _blockUserAction;
        private readonly ILogger<UserController> _logger;

        public UserController(IBlockUserAction blockUserAction, ILogger<UserController> logger)
        {
            _blockUserAction = blockUserAction;
            _logger = logger;
        }

        [HttpPost]
        [BlockUserRequestValidator]
        public IActionResult BlockUser(BlockUserRequest blockUserRequest)
        {
            string currentUsername = GetCurrentUsername();

            var userBlockingProcessResult = _blockUserAction.BlockUser(new UserBlockingContext { CurrentUsername = currentUsername, BlockUserRequest = blockUserRequest });
            _logger.LogInformation($"{blockUserRequest.BlockedUsername} blocked by {currentUsername}");

            return Ok(userBlockingProcessResult);
        }

        private string GetCurrentUsername()
            => User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}