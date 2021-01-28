using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
    public class MessageHubController : ControllerBase
    {
        private readonly ILogger<MessageHubController> _logger;
        private readonly IMessageService _messageService;
        private readonly IMessageHubService _messageHubService;
        private readonly IBlockUserAction _blockUserAction;

        public MessageHubController(ILogger<MessageHubController> logger, IMessageService messageService, IMessageHubService messageHubService, IBlockUserAction blockUserAction)
        {
            _logger = logger;
            _messageService = messageService;
            _messageHubService = messageHubService;
            _blockUserAction = blockUserAction;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessageHistory(string userName)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrWhiteSpace(userName))
                return BadRequest(new JsonResult(new ValidationResult { IsValid = false, Message = Constants.ValidationMessages.StringCanNotBeNullEmptyOrWhiteSpace }));

            return Ok(await _messageService.GetMessagesBetweenTwoUser(GetCurrentUsername(), userName));
        }

        [HttpGet]
        [Authorize(Roles = Model.Constants.MessageHub.Role.Admin)]
        public IActionResult GetConnectedUsernames()
            => Ok(_messageHubService.GetConnectedUsernames());

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