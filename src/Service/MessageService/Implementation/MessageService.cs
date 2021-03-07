using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessagingService.Data;
using MessagingService.Model;

namespace MessagingService.Service
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMessageGroupRepository _messageGroupRepository;
        private readonly IMessageHubService _messageHubService;

        public MessageService(IMessageRepository messageRepository, IMessageGroupRepository messageGroupRepository, IMessageHubService messageHubService)
        {
            _messageRepository = messageRepository;
            _messageGroupRepository = messageGroupRepository;
            _messageHubService = messageHubService;
        }

        public async Task<List<Message>> GetMessagesBetweenTwoUser(string userName1, string userName2)
        {
            var messages = await _messageRepository.GetList(m =>
                (m.SenderUsername == userName1 && m.ReceiverUsername == userName2)
                    ||
                (m.SenderUsername == userName2 && m.ReceiverUsername == userName1));

            return messages.OrderBy(m => m.TimeToSend)?.ToList();
        }

        public async Task SaveMessage(Message message)
            => await _messageRepository.Create(message);

        public async Task SaveMessageGroup(MessageGroupSaveContext context)
        {
            object savedMessageGroupId;

            if (context.TransactionType == TransactionType.Insert)
            {
                savedMessageGroupId = await _messageGroupRepository.Create(new MessageGroup
                {
                    GroupName = context.CreationContext.GroupName,
                    AdminUsername = context.CreationContext.AdminUsername
                });
            }
            else
            {
                savedMessageGroupId = await _messageGroupRepository.FindAndUpdate(mg => mg.GroupName == context.UpdateContext.GroupName && mg.AdminUsername == mg.AdminUsername,
                    GetUpdateActionByUpdateType(context.UpdateContext.UpdateType, context.UpdateContext.Username));
            }
            context.MessageGroupId = savedMessageGroupId.ToString();
        }

        private Action<MessageGroup> GetUpdateActionByUpdateType(MessageGroupUpdateType transactionType, string userName)
            => transactionType == MessageGroupUpdateType.AdditionToGroup
                ? (mg) => mg.UsernamesInGroup.Add(userName)
                : (mg) => mg.UsernamesInGroup.Remove(userName);
    }
}