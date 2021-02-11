using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MessagingService.Model;
using MessagingService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessagingService.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class MessageHubController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IMessageHubService _messageHubService;

        public MessageHubController(IMessageService messageService, IMessageHubService messageHubService)
        {
            _messageService = messageService;
            _messageHubService = messageHubService;
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
        public IActionResult GetMessageHubState()
            => Ok(Hubs.MessageHubState.ConnectedUsers);

        private string GetCurrentUsername()
            => User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}