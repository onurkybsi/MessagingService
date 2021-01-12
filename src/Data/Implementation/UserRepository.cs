using MessagingService.Infrastructure;
using MessagingService.Model;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;

namespace MessagingService.Data
{
    public class UserRepository : MongoDBCollectionBase<User>, IUserRepository
    {
        public UserRepository(MongoDBCollectionSettings settings) : base(settings)
        {
            CreateUniqueIndexForUsername();
        }

        private void CreateUniqueIndexForUsername()
        {
            var keys = Builders<User>.IndexKeys.Text(u => u.Username);
            var indexOptions = new CreateIndexOptions { Name = "Unique_Username", Unique = true, Sparse = true };
            var model = new CreateIndexModel<User>(keys, indexOptions);
            _collection.Indexes.CreateOne(model);
        }

        public async Task<TField> GetSpecifiedFieldByUsername<TField>(string username, Expression<Func<User, TField>> fieldExpression)
        {
            var projectionThatGetSpecifiedField = Builders<User>.Projection.Expression(fieldExpression);

            return await _collection.Find(u => u.Username == username).Project(projectionThatGetSpecifiedField).FirstOrDefaultAsync();
        }
    }
}