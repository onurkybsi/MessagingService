using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MessagingService.Infrastructure
{
    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
        List<T> GetList(Expression<Func<T, bool>> filter = null);
    }
}