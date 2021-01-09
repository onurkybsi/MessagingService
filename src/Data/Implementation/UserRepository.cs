using MessagingService.Infrastructure;
using MessagingService.Model;

namespace MessagingService.Data
{
    public class UserRepository : MongoDBCollectionBase<User>, IUserRepository
    {
        public UserRepository(MongoDBCollectionSettings settings) : base(settings) { }
    }
}