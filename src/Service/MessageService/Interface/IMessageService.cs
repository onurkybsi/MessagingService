using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MessagingService.Model;

namespace MessagingService.Service
{
    public interface IMessageService
    {
        Task<List<Message>> GetMessages(Expression<Func<Message, bool>> filter);
        Task SaveMessage(Message message);
    }
}