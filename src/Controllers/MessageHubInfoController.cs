using MessagingService.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MessagingService.Controllers
{
    [Authorize(Roles = Model.Constants.MessageHub.Role.Admin)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MessageHubInfoController : ControllerBase
    {
        private readonly ILogger<MessageHubInfoController> _logger;

        public MessageHubInfoController(ILogger<MessageHubInfoController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetConnectedUsernames()
            => Ok(MessageHubState.Usernames);
    }
}