using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace MessagingService.Infrastructure
{
    public class MongoDBDriverRepositoryBase<TEntity> : IEntityRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<TEntity> _collection;


        public MongoDBDriverRepositoryBase(IRepositorySettings repositorySettings)
        {
            _mongoClient = new MongoClient(repositorySettings.ConnectionString);
            _database = _mongoClient.GetDatabase(repositorySettings.DatabaseName);
            _collection = _database.GetCollection<TEntity>(repositorySettings.CollectionName);
        }

        public List<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null)
            => filter is null ? _collection.Find(document => true).ToList() : _collection.Find(new ExpressionFilterDefinition<TEntity>(filter)).ToList();
    }
}