using MessagingService.Infrastructure;
using MessagingService.Model;

namespace MessagingService.Data {
  public class MessageGroupRepository : MongoDBCollectionBase<MessageGroup>, IMessageGroupRepository {
    public MessageGroupRepository(MongoDBCollectionSettings settings) : base(settings) { }
  }
}