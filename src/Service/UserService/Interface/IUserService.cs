using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MessagingService.Model;

namespace MessagingService.Service
{
    public interface IUserService
    {
        Task<User> GetUser(Expression<Func<User, bool>> filter);
        Task<List<User>> GetUsers(Expression<Func<User, bool>> filter);
        Task CreateUser(User user);
    }
}