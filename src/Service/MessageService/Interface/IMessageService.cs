using System.Collections.Generic;
using System.Threading.Tasks;
using MessagingService.Action;
using MessagingService.Model;

namespace MessagingService.Service
{
    public interface IMessageService : ISaveMessageGroupAsync
    {
        Task<List<Message>> GetMessagesBetweenTwoUser(string userName1, string userName2);
        Task SaveMessage(Message message);
    }
}