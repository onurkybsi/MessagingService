using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MessagingService.Infrastructure {
  public abstract class MongoDBCollectionBase<T> : IEntityRepository<T> where T : MongoDBEntity, new() {
    protected readonly IMongoCollection<T> _collection;

    public MongoDBCollectionBase(MongoDBCollectionSettings collectionSettings) {
      var client = new MongoClient(collectionSettings.DatabaseSettings.ConnectionString);
      var database = client.GetDatabase(collectionSettings.DatabaseSettings.DatabaseName);

      _collection = database.CreateCollectionIfNotExists<T>(collectionSettings.CollectionName, collectionSettings.CreateCollectionOptions);
    }

    public async Task<T> Get(Expression<Func<T, bool>> filter)
        => filter != null ? (await _collection.FindAsync(new ExpressionFilterDefinition<T>(filter))).FirstOrDefault() : null;

    public async Task<List<T>> GetList(Expression<Func<T, bool>> filter = null)
        => filter is null ? (await _collection.FindAsync(document => true)).ToList() : (await _collection.FindAsync(new ExpressionFilterDefinition<T>(filter))).ToList();

    public async Task<object> Create(T entity) {
      entity.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
      await _collection.InsertOneAsync(entity);
      return entity.Id;
    }

    public Task Update(T entity)
        => string.IsNullOrEmpty(entity.Id) ? Task.CompletedTask : _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);

    // It will be changed. This may not be optimal solution
    public async Task<object> FindAndUpdate(Expression<Func<T, bool>> filterDefinition, Action<T> updateDefinition) {
      T updatedEntity = (await _collection.FindAsync(new ExpressionFilterDefinition<T>(filterDefinition))).FirstOrDefault();
      if (updatedEntity == default(T)) {
        throw new Exception("Expressed entity does not exist");
      }
      updateDefinition(updatedEntity);
      await this.Update(updatedEntity);

      return updatedEntity.Id;
    }

    public async Task FindAndUpdate(Expression<Func<T, bool>> filterDefinition, Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>> updateDefinition) {
      var builder = Builders<T>.Update;
      await _collection.UpdateOneAsync(filterDefinition, updateDefinition(builder));
    }

    public Task Remove(T entity)
        => _collection.DeleteOneAsync(e => e.Id == entity.Id);
  }
}