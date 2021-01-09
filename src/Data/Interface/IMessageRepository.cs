using MessagingService.Infrastructure;
using MessagingService.Model;

namespace MessagingService.Data
{
    public interface IMessageRepository : IEntityRepository<Message> { }
}