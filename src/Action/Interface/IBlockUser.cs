using System.Threading.Tasks;
using MessagingService.Model;

namespace MessagingService.Action {
  public interface IBlockUser {
    void BlockUser(UserBlockingContext context);
  }

  public interface IBlockUserAsync {
    Task BlockUser(UserBlockingContext context);
  }
}