using System.Threading.Tasks;

namespace MessagingService.Model
{
    public interface IMessageClient
    {
        Task ReceiveMessage(string message);
    }
}