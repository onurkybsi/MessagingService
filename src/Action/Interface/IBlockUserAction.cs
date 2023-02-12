using MessagingService.Infrastructure;
using MessagingService.Model;

namespace MessagingService.Action {
  public interface IBlockUserAction {
    ProcessResult BlockUser(UserBlockingContext context);
  }
}