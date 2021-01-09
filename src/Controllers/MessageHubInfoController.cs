using System.Linq;
using System.Security.Claims;
using MessagingService.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MessagingService.Controllers
{
    [Authorize]
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
            => Ok(MessageHubState.Usernames.Where(user => user != GetCurrentUsername()));

        private string GetCurrentUsername()
            => User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}