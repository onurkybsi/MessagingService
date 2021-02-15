using System.Threading.Tasks;
using MessagingService.Model;

namespace MessagingService.Action
{
    public interface ISaveMessageGroup
    {
        void SaveMessageGroup(MessageGroupSaveContext context);
        // TO-DO: Not true architecture change it
        bool MustBeExecuteFirst { get; }
    }

    public interface ISaveMessageGroupAsync
    {
        Task SaveMessageGroup(MessageGroupSaveContext context);
        // TO-DO: Not true architecture change it
        bool MustBeExecuteFirst { get; }
    }
}