using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MessagingService.Infrastructure
{
    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
        Task<T> Get(Expression<Func<T, bool>> filter);
        Task<List<T>> GetList(Expression<Func<T, bool>> filter = null);
        /// <summary>
        /// It create entity, return Id of created entity 
        /// </summary>
        Task<object> Create(T entity);
        Task Update(T entity);
        /// <summary>
        /// It find to be updated entity, update it, return Id of updated entity 
        /// </summary>
        Task<object> FindAndUpdate(Expression<Func<T, bool>> filterDefinition, Action<T> updateDefinition);
        Task Remove(T entity);
    }
}