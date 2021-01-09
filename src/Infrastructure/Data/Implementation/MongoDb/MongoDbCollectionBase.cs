using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MessagingService.Infrastructure
{
    public abstract class MongoDBCollectionBase<T> : IEntityRepository<T> where T : MongoDBEntity, new()
    {
        protected readonly IMongoCollection<T> _collection;

        public MongoDBCollectionBase(MongoDBCollectionSettings collectionSettings)
        {
            var client = new MongoClient(collectionSettings.DatabaseSettings.ConnectionString);
            var database = client.GetDatabase(collectionSettings.DatabaseSettings.DatabaseName);

            _collection = database.GetCollection<T>(collectionSettings.CollectionName);
        }

        public async Task<T> Get(Expression<Func<T, bool>> filter)
            => filter != null ? (await _collection.FindAsync(new ExpressionFilterDefinition<T>(filter))).FirstOrDefault() : null;

        public async Task<List<T>> GetList(Expression<Func<T, bool>> filter = null)
            => filter is null ? (await _collection.FindAsync(document => true)).ToList() : (await _collection.FindAsync(new ExpressionFilterDefinition<T>(filter))).ToList();

        public async Task Create(T entity)
            => await _collection.InsertOneAsync(entity);

        public Task Update(T entity)
            => string.IsNullOrEmpty(entity.Id) ? Task.CompletedTask : _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);

        public Task Remove(T entity)
            => _collection.DeleteOneAsync(e => e.Id == entity.Id);
    }
}