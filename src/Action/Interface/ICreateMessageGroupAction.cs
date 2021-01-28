using MessagingService.Infrastructure;
using MessagingService.Model;

namespace MessagingService.Action
{
    public interface ICreateMessageGroupAction
    {
        ProcessResult CreateMessageGroup(MessageGroupCreationContext context);
    }
}