using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MessagingService.Data;
using MessagingService.Model;

namespace MessagingService.Service
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<List<Message>> GetMessages(Expression<Func<Message, bool>> filter)
            => await _messageRepository.GetList(filter);

        public async Task SaveMessage(Message message)
            => await _messageRepository.Create(message);
    }
}