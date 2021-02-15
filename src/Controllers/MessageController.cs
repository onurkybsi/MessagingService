using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MessagingService.Infrastructure;
using MessagingService.Model;
using MessagingService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessagingService.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class MessageController : ControllerBaseExtra
    {
        private readonly IMessageService _messageService;
        private readonly Action.ISaveMessageGroupAction _saveMessageGroupAction;

        public MessageController(IMessageService messageService, Action.ISaveMessageGroupAction saveMessageGroupAction)
        {
            _messageService = messageService;
            _saveMessageGroupAction = saveMessageGroupAction;
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

        [HttpPost]
        public IActionResult CreateMessageGroup([FromBody] MessageGroupCreationContext context)
        {
            ProcessResult messageGroupCreationResult = _saveMessageGroupAction.SaveMessageGroup(new MessageGroupSaveContext
            {
                SaveType = SaveType.Insert,
                CreationContext = new MessageGroupCreationContext { AdminUsername = GetCurrentUsername(), GroupName = context.GroupName }
            });

            return Ok(messageGroupCreationResult);
        }

        [HttpPost]
        public IActionResult UpdateMessageGroup([FromBody] MessageGroupUpdateContext context)
        {
            ProcessResult messageGroupUpdateResult = _saveMessageGroupAction.SaveMessageGroup(new MessageGroupSaveContext
            {
                SaveType = SaveType.Update,
                UpdateContext = new MessageGroupUpdateContext { AddedUsername = context.AddedUsername, GroupName = context.GroupName }
            });

            return Ok(messageGroupUpdateResult);
        }
    }
}