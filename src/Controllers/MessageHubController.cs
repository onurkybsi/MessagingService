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
        [Authorize(Roles = Model.Constants.MessageHub.Role.Admin)]
        public IActionResult GetMessageHubState()
            => Ok(Hubs.MessageHubState.ConnectedUsers);
    }
}