using System;
using System.Collections.Generic;
using System.Linq;
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
            => (await _messageRepository.GetList(filter)).OrderBy(m => m.TimeToSend).ToList();

        public async Task<List<Message>> GetMessagesBetweenTwoUser(string userName1, string userName2)
        {
            var messages = await _messageRepository.GetList(m =>
                (m.SenderUsername == userName1 && m.SenderUsername == userName2)
                    ||
                (m.SenderUsername == userName2 && m.SenderUsername == userName1));

            return messages.OrderBy(m => m.TimeToSend).ToList();
        }


        public async Task SaveMessage(Message message)
            => await _messageRepository.Create(message);
    }
}