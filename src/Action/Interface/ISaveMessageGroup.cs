using System.Threading.Tasks;
using MessagingService.Model;

namespace MessagingService.Action {
  public interface ISaveMessageGroup {
    void SaveMessageGroup(MessageGroupSaveContext context);
  }

  public interface ISaveMessageGroupAsync {
    Task SaveMessageGroup(MessageGroupSaveContext context);
  }
}