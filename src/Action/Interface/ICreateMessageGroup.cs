using System.Threading.Tasks;
using MessagingService.Model;

namespace MessagingService.Action
{
    public interface ICreateMessageGroup
    {
        void CreateMessageGroup(MessageGroupCreationContext context);
    }

    public interface ICreateMessageGroupAsync
    {
        Task CreateMessageGroup(MessageGroupCreationContext context);
    }
}