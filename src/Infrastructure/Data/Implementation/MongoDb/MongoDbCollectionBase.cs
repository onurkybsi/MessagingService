using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace MessagingService.Infrastructure
{
    public abstract class MongoDbCollectionBase<T> : IEntityRepository<T> where T : class, IEntity, new()
    {
        protected readonly IMongoCollection<T> _collection;

        public MongoDbCollectionBase(MongoDbCollectionSettings collectionSettings)
        {
            var client = new MongoClient(collectionSettings.DatabaseSettings.ConnectionString);
            var database = client.GetDatabase(collectionSettings.DatabaseSettings.DatabaseName);

            _collection = database.GetCollection<T>(collectionSettings.CollectionName);
        }

        public List<T> GetList(Expression<Func<T, bool>> filter = null)
            => filter is null ? _collection.Find(document => true).ToList() : _collection.Find(new ExpressionFilterDefinition<T>(filter)).ToList();
    }
}