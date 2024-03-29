using MessagingService.Infrastructure;
using MessagingService.Model;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;

namespace MessagingService.Data {
  public class UserRepository : MongoDBCollectionBase<User>, IUserRepository {
    public UserRepository(MongoDBCollectionSettings settings) : base(settings) {
      CreateUniqueIndexForUsername();
    }

    private void CreateUniqueIndexForUsername() {
      var indexOptions = new CreateIndexOptions { Name = "Unique_Username", Unique = true, Sparse = true };
      var model = new CreateIndexModel<User>(new BsonDocument("Username", 1), indexOptions);
      _collection.Indexes.CreateOne(model);
    }

    public async Task<TField> GetSpecifiedFieldByUsername<TField>(string username, Expression<Func<User, TField>> fieldExpression) {
      var projectionThatGetSpecifiedField = Builders<User>.Projection.Expression(fieldExpression);

      return await _collection.Find(u => u.Username == username).Project(projectionThatGetSpecifiedField).FirstOrDefaultAsync();
    }
  }
}