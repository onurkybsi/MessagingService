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
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMessage()
        {
            // TO-DO: Gruplarla bitlikte current user ın tüm mesaj historysini dönmeli.
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetMessageHistoryWithTheUser(string userName)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrWhiteSpace(userName))
                return BadRequest(new JsonResult(new Model.ValidationResult { IsValid = false, Message = Constants.ValidationMessages.StringCanNotBeNullEmptyOrWhiteSpace }));

            return Ok(await _messageService.GetMessagesBetweenTwoUser(GetCurrentUsername(), userName));
        }

        private string GetCurrentUsername()
            => User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}