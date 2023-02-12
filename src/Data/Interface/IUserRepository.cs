using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MessagingService.Infrastructure;
using MessagingService.Model;

namespace MessagingService.Data {
  public interface IUserRepository : IEntityRepository<User> {
    Task<TField> GetSpecifiedFieldByUsername<TField>(string username, Expression<Func<User, TField>> fieldExpression);
  }
}