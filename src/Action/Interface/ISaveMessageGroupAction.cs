using MessagingService.Infrastructure;
using MessagingService.Model;

namespace MessagingService.Action {
  public interface ISaveMessageGroupAction {
    ProcessResult SaveMessageGroup(MessageGroupSaveContext context);
  }
}