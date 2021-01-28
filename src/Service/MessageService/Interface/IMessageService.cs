using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MessagingService.Action;
using MessagingService.Model;

namespace MessagingService.Service
{
    public interface IMessageService : ICreateMessageGroupAsync
    {
        Task<List<Message>> GetMessages(Expression<Func<Message, bool>> filter);
        Task<List<Message>> GetMessagesBetweenTwoUser(string userName1, string userName2);
        Task SaveMessage(Message message);
    }
}