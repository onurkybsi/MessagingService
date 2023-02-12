using System.Collections.Generic;
using MessagingService.Action;

namespace MessagingService.Service {
  public interface IMessageHubService : IBlockUser, ISaveMessageGroupAsync {
    List<string> GetConnectedUsernames();
  }
}