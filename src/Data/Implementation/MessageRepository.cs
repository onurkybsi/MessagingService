using MessagingService.Infrastructure;
using MessagingService.Model;

namespace MessagingService.Data {
  public class MessageRepository : MongoDBCollectionBase<Message>, IMessageRepository {
    public MessageRepository(MongoDBCollectionSettings settings) : base(settings) { }
  }
}