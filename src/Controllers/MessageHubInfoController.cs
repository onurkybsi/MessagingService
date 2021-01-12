using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MessagingService.Hubs;
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
    public class MessageHubInfoController : ControllerBase
    {
        private readonly ILogger<MessageHubInfoController> _logger;
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;

        public MessageHubInfoController(ILogger<MessageHubInfoController> logger, IMessageService messageService, IUserService userService)
        {
            _logger = logger;
            _messageService = messageService;
            _userService = userService;
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
            => Ok(MessageHubState.ConnectedUsernames);

        [HttpPost]
        public async Task<IActionResult> BlockUser(BlockUserRequest blockUserRequest)
        {
            string currentUsername = GetCurrentUsername();

            MessageHubState.BlockedUsersInfo.Where(bi => bi.Key == currentUsername).First().Value.Add(blockUserRequest.BlockedUsername);

            await _userService.UpdateByUsername(currentUsername, u => { u.BlockedUsers.Add(blockUserRequest.BlockedUsername); });
            _logger.LogInformation($"{blockUserRequest.BlockedUsername} blocked by {currentUsername}");

            return Ok();
        }

        private string GetCurrentUsername()
            => User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}