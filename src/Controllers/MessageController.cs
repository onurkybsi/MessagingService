using System.Threading.Tasks;
using MessagingService.Infrastructure;
using MessagingService.Model;
using MessagingService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MessagingService.Controllers {
  [Route("api/[controller]/[action]")]
  [Authorize]
  [ApiController]
  public class MessageController : ControllerBaseExtra {
    private readonly IMessageService _messageService;
    private readonly Action.ISaveMessageGroupAction _saveMessageGroupAction;

    public MessageController(IMessageService messageService, Action.ISaveMessageGroupAction saveMessageGroupAction) {
      _messageService = messageService;
      _saveMessageGroupAction = saveMessageGroupAction;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMessage() {
      // TO-DO: Gruplarla bitlikte current user ın tüm mesaj historysini dönmeli.
      return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetMessageHistoryWithTheUser(string userName) {
      if (string.IsNullOrEmpty(userName) || string.IsNullOrWhiteSpace(userName))
        return BadRequest(new JsonResult(new ValidationResult { IsValid = false, Message = Constants.ValidationMessages.StringCanNotBeNullEmptyOrWhiteSpace }));

      return Ok(await _messageService.GetMessagesBetweenTwoUser(GetCurrentUsername(), userName));
    }

    [HttpPost]
    [MessageGroupCreationContextValidator]
    public IActionResult CreateMessageGroup([FromBody] MessageGroupCreationContext context) {
      ProcessResult messageGroupCreationResult = _saveMessageGroupAction.SaveMessageGroup(new MessageGroupSaveContext(GetCurrentUsername(), context.GroupName));
      return Ok(messageGroupCreationResult);
    }

    [HttpPut]
    [MessageGroupUpdateContextValidator]
    public IActionResult AddUserToMessageGroup([FromBody] MessageGroupUpdateContext context) {
      ProcessResult messageGroupUpdateResult = _saveMessageGroupAction.SaveMessageGroup(
          new MessageGroupSaveContext(GetCurrentUsername(), context.MessageGroupId, context.Username, MessageGroupUpdateType.AdditionToGroup)
      );
      return Ok(messageGroupUpdateResult);
    }

    [HttpDelete]
    [MessageGroupUpdateContextValidator]
    public IActionResult DeleteUserFromMessageGroup([FromBody] MessageGroupUpdateContext context) {
      ProcessResult messageGroupUpdateResult = _saveMessageGroupAction.SaveMessageGroup(
          new MessageGroupSaveContext(GetCurrentUsername(), context.MessageGroupId, context.Username, MessageGroupUpdateType.EliminationFromGroup)
      );
      return Ok(messageGroupUpdateResult);
    }
  }
}