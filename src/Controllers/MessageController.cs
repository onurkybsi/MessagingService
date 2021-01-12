using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MessagingService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MessagingService.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private readonly IMessageService _messageService;

        public MessageController(ILogger<MessageController> logger, IMessageService messageService)
        {
            _logger = logger;
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessageHistory(string userName)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrWhiteSpace(userName))
                return BadRequest();

            return Ok(await _messageService.GetMessagesBetweenTwoUser(GetCurrentUsername(), userName));
        }

        private string GetCurrentUsername()
            => User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}